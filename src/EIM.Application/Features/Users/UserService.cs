using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Users;

public interface IUserService
{
    Task<PagedResult<UserDto>> ListAsync(UserQuery query, CancellationToken ct);
    Task<UserDto> GetAsync(Guid id, CancellationToken ct);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct);
    Task<UserDto> AssignRoleAsync(Guid id, AssignRoleRequest request, CancellationToken ct);
    Task<UserDto> RevokeRoleAsync(Guid id, string roleCode, CancellationToken ct);
    Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct);
    Task<IReadOnlyList<TeacherDto>> ListTeachersAsync(CancellationToken ct);
}

internal sealed class UserService(
    IAppDbContext db,
    IPasswordHasher hasher,
    RoleDirectory roles,
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
            var ids = await roles.UsersInRolesAsync([role.ToUpperInvariant()], ct);
            users = users.Where(u => ids.Contains(u.Id));
        }

        var page = await users.OrderByDescending(u => u.CreatedAt).ToPagedAsync(query, ct);
        var map = await roles.RolesAsync(page.Items.Select(u => u.Id).ToList(), ct);
        return new PagedResult<UserDto>(page.Items.Select(u => ToDto(u, map)).ToList(), page.Page, page.PageSize, page.Total);
    }

    public async Task<UserDto> GetAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct) ?? throw AppException.NotFound("người dùng");
        return ToDto(user, await roles.RolesAsync([id], ct));
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw AppException.Conflict("EMAIL_EXISTS", "Email đã được sử dụng");
        var codes = await ValidateRolesAsync(request.Roles, ct);

        var user = new AuthUser { Email = email, FullName = request.FullName.Trim(), PasswordHash = hasher.Hash(request.Password) };
        db.Users.Add(user);
        audit.Record(AuditActions.Create, AuditEntities.User, user.Id, new { user.Email, user.FullName, Roles = codes });
        foreach (var code in codes) roles.Assign(user.Id, code);
        await db.SaveChangesAsync(ct);
        return await GetAsync(user.Id, ct);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct) ?? throw AppException.NotFound("người dùng");
        var status = Guard.OneOf(request.Status, UserStatuses.All, "Trạng thái");
        if (id == currentUser.RequiredId && status != UserStatuses.Active)
            throw AppException.Conflict("SELF_DISABLE", "Không thể tự khóa tài khoản của mình");

        var before = new { user.FullName, user.Status };
        user.FullName = request.FullName.Trim();
        user.Status = status;
        if (request.Password is not null) user.PasswordHash = hasher.Hash(request.Password);
        audit.Record(AuditActions.Update, AuditEntities.User, id, new
        {
            Before = before,
            After = new { user.FullName, user.Status },
            PasswordChanged = request.Password is not null
        });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<UserDto> AssignRoleAsync(Guid id, AssignRoleRequest request, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(u => u.Id == id, ct)) throw AppException.NotFound("người dùng");
        var code = (await ValidateRolesAsync([request.RoleCode], ct))[0];
        if ((await roles.RolesOfAsync(id, ct)).Contains(code))
            throw AppException.Conflict("ROLE_ASSIGNED", "Người dùng đã có vai trò này");

        roles.Assign(id, code);
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<UserDto> RevokeRoleAsync(Guid id, string roleCode, CancellationToken ct)
    {
        var code = roleCode.Trim().ToUpperInvariant();
        if (id == currentUser.RequiredId && code == RoleCodes.Root)
            throw AppException.Conflict("SELF_REVOKE_ROOT", "Không thể tự thu hồi vai trò ROOT của mình");
        if (!(await roles.RolesOfAsync(id, ct)).Contains(code))
            throw AppException.NotFound("vai trò của người dùng");

        roles.Revoke(id, code);
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct)
    {
        var map = await roles.RolesAsync(null, ct);
        var all = await db.Roles.AsNoTracking().OrderBy(r => r.Code).ToListAsync(ct);
        return all.Select(r => new RoleDto(r.Id, r.Code, r.Name, map.Values.Count(v => v.Contains(r.Code)))).ToList();
    }

    public async Task<IReadOnlyList<TeacherDto>> ListTeachersAsync(CancellationToken ct)
    {
        var ids = await roles.UsersInRolesAsync([RoleCodes.Teacher], ct);
        return await db.Users.AsNoTracking()
            .Where(u => ids.Contains(u.Id) && u.Status == UserStatuses.Active)
            .OrderBy(u => u.FullName)
            .Select(u => new TeacherDto(u.Id, u.FullName, u.Email))
            .ToListAsync(ct);
    }

    private async Task<IReadOnlyList<string>> ValidateRolesAsync(IEnumerable<string> requested, CancellationToken ct)
    {
        var codes = requested.Select(r => r.Trim().ToUpperInvariant()).Distinct().ToList();
        if (await db.Roles.CountAsync(r => codes.Contains(r.Code), ct) != codes.Count)
            throw AppException.Invalid("INVALID_ROLE", "Có vai trò không tồn tại");
        return codes;
    }

    private static UserDto ToDto(AuthUser user, IReadOnlyDictionary<Guid, IReadOnlyList<string>> map) =>
        new(user.Id, user.Email, user.FullName, user.Status, user.CreatedAt, map.GetValueOrDefault(user.Id) ?? []);
}
