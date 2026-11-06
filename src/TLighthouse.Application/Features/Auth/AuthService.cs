using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct);
    Task LogoutAsync(CancellationToken ct);
    Task<CurrentUserDto> GetCurrentAsync(CancellationToken ct);
}

internal sealed class AuthService(
    IAppDbContext db,
    IPasswordHasher hasher,
    RoleDirectory roles,
    ITokenService tokens,
    ICurrentUser currentUser,
    IAuditTrail audit) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await LoadAsync(u => u.Email == email, ct);
        if (user is null || !hasher.Verify(user.PasswordHash, request.Password))
            throw AppException.Unauthorized("Email hoặc mật khẩu không đúng");
        if (user.Status != UserStatuses.Active)
            throw AppException.Forbidden("Tài khoản đã bị khóa");
        return await IssueAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        var payload = await tokens.ReadRefreshTokenAsync(request.RefreshToken)
                      ?? throw AppException.Unauthorized("Refresh token không hợp lệ hoặc đã hết hạn");
        var user = await LoadAsync(u => u.Id == payload.UserId, ct);
        if (user is null || user.Status != UserStatuses.Active)
            throw AppException.Unauthorized("Tài khoản không còn hoạt động");
        var loggedOut = await db.AuditLogs.AnyAsync(a =>
            a.ActorUserId == user.Id && a.Action == AuditActions.Logout && a.CreatedAt >= payload.IssuedAt, ct);
        if (loggedOut)
            throw AppException.Unauthorized("Phiên làm việc đã đăng xuất");
        return await IssueAsync(user, ct);
    }

    public async Task LogoutAsync(CancellationToken ct)
    {
        audit.Record(AuditActions.Logout, AuditEntities.User, currentUser.RequiredId);
        await db.SaveChangesAsync(ct);
    }

    public async Task<CurrentUserDto> GetCurrentAsync(CancellationToken ct)
    {
        var id = currentUser.RequiredId;
        var user = await LoadAsync(u => u.Id == id, ct) ?? throw AppException.NotFound("người dùng");
        return await ToDtoAsync(user, ct);
    }

    private Task<AuthUser?> LoadAsync(System.Linq.Expressions.Expression<Func<AuthUser, bool>> predicate, CancellationToken ct) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    private async Task<AuthResponse> IssueAsync(AuthUser user, CancellationToken ct)
    {
        var dto = await ToDtoAsync(user, ct);
        if (dto.Roles.Count == 0) throw AppException.Forbidden("Tài khoản chưa được gán vai trò");
        return new AuthResponse(tokens.Issue(user, dto.Roles), dto);
    }

    private async Task<CurrentUserDto> ToDtoAsync(AuthUser user, CancellationToken ct) =>
        new(user.Id, user.Email, user.FullName, user.Status, await roles.RolesOfAsync(user.Id, ct));
}
