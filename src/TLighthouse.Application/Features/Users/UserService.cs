using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Users;

public interface IUserService
{
    Task<PagedResult<UserDto>> ListAsync(UserQuery query, CancellationToken ct);
    Task<UserDto> GetAsync(Guid id, CancellationToken ct);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct);
    Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct);
    Task<IReadOnlyList<TeacherDto>> ListTeachersAsync(CancellationToken ct);
}

internal sealed class UserService(
    IAppDbContext db,
    IPasswordHasher hasher,
    ICurrentUser currentUser,
    IAuditTrail audit) : IUserService
{
    public async Task<PagedResult<UserDto>> ListAsync(UserQuery query, CancellationToken ct)
    {
        var users = db.Users.AsNoTracking();
        if (query.Search.Clean() is { } search)
            users = users.Where(u => u.Email.Contains(search) || u.FullName.Contains(search));
        if (query.Status.Clean() is { } status)
            users = users.Where(u => u.Status == status.ToUpper());
        if (query.Role.Clean() is { } role)
        {
            var code = role.ToUpperInvariant();
            users = users.Where(u => u.Role == code);
        }

        var page = await users.OrderByDescending(u => u.CreatedAt).ToPagedAsync(query, ct);
        return new PagedResult<UserDto>(page.Items.Select(ToDto).ToList(), page.Page, page.PageSize, page.Total);
    }

    public async Task<UserDto> GetAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct) ?? throw AppException.NotFound("người dùng");
        return ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw AppException.Conflict("EMAIL_EXISTS", "Email đã được sử dụng");
        var role = ValidateRole(request.Role);

        var user = new AuthUser { Email = email, FullName = request.FullName.Trim(), PasswordHash = hasher.Hash(request.Password), Role = role };
        db.Users.Add(user);
        audit.Record(AuditActions.Create, AuditEntities.User, user.Id, new { user.Email, user.FullName, user.Role });
        await db.SaveChangesAsync(ct);
        return await GetAsync(user.Id, ct);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct) ?? throw AppException.NotFound("người dùng");
        var status = Guard.OneOf(request.Status, UserStatuses.All, "Trạng thái");
        if (id == currentUser.RequiredId && status != UserStatuses.Active)
            throw AppException.Conflict("SELF_DISABLE", "Không thể tự khóa tài khoản của mình");

        var role = ValidateRole(request.Role);
        if (id == currentUser.RequiredId && user.Role == RoleCodes.Root && role != RoleCodes.Root)
            throw AppException.Conflict("SELF_REVOKE_ROOT", "Không thể tự thu hồi vai trò ROOT của mình");

        var before = new { user.FullName, user.Status, user.Role };
        var roleChanged = user.Role != role;
        user.FullName = request.FullName.Trim();
        user.Status = status;
        user.Role = role;
        if (request.Password is not null) user.PasswordHash = hasher.Hash(request.Password);
        audit.Record(AuditActions.Update, AuditEntities.User, id, new
        {
            Before = before,
            After = new { user.FullName, user.Status, user.Role },
            PasswordChanged = request.Password is not null
        });
        if (roleChanged) audit.Record(AuditActions.AssignRole, AuditEntities.User, id, new { Role = role });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct)
    {
        var counts = await db.Users.AsNoTracking().GroupBy(u => u.Role).Select(g => new { Role = g.Key, Count = g.Count() }).ToListAsync(ct);
        return RoleCodes.Names
            .OrderBy(r => r.Key)
            .Select(r => new RoleDto(r.Key, r.Value, counts.FirstOrDefault(c => c.Role == r.Key)?.Count ?? 0))
            .ToList();
    }

    public async Task<IReadOnlyList<TeacherDto>> ListTeachersAsync(CancellationToken ct)
    {
        return await db.Users.AsNoTracking()
            .Where(u => u.Role == RoleCodes.Teacher && u.Status == UserStatuses.Active)
            .OrderBy(u => u.FullName)
            .Select(u => new TeacherDto(u.Id, u.FullName, u.Email))
            .ToListAsync(ct);
    }

    private static string ValidateRole(string requested)
    {
        var code = requested.Trim().ToUpperInvariant();
        if (!RoleCodes.Names.ContainsKey(code))
            throw AppException.Invalid("INVALID_ROLE", "Vai trò không tồn tại");
        return code;
    }

    private static UserDto ToDto(AuthUser user) =>
        new(user.Id, user.Email, user.FullName, user.Status, user.CreatedAt, user.Role);
}
