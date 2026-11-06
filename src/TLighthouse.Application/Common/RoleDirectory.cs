using TLighthouse.Application.Abstractions;
using TLighthouse.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Common;

internal sealed class RoleDirectory(IAppDbContext db)
{
    public async Task<IReadOnlyList<Guid>> UsersInRolesAsync(IReadOnlyCollection<string> roleCodes, CancellationToken ct) =>
        await db.Users.AsNoTracking().Where(u => roleCodes.Contains(u.Role)).Select(u => u.Id).ToListAsync(ct);

    public async Task EnsureActiveTeachersAsync(CancellationToken ct, params Guid?[] userIds)
    {
        var ids = userIds.OfType<Guid>().Distinct().ToList();
        if (ids.Count == 0) return;
        var teachers = await db.Users
            .Where(u => ids.Contains(u.Id) && u.Status == UserStatuses.Active && u.Role == RoleCodes.Teacher)
            .CountAsync(ct);
        if (teachers != ids.Count)
            throw AppException.Invalid("INVALID_TEACHER", "Người dùng được chọn không phải giáo viên đang hoạt động");
    }
}
