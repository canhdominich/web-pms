using EIM.Application.Abstractions;
using EIM.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Common;

internal sealed record RoleMeta(string Role);

internal sealed class RoleDirectory(IAppDbContext db, IAuditTrail audit)
{
    private static readonly string[] Actions = [AuditActions.AssignRole, AuditActions.RevokeRole];

    public void Assign(Guid userId, string role) => audit.Record(AuditActions.AssignRole, AuditEntities.User, userId, new RoleMeta(role));

    public void Revoke(Guid userId, string role) => audit.Record(AuditActions.RevokeRole, AuditEntities.User, userId, new RoleMeta(role));

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> RolesAsync(IReadOnlyCollection<Guid>? userIds, CancellationToken ct)
    {
        var query = db.AuditLogs.AsNoTracking().Events(AuditEntities.User, Actions);
        if (userIds is not null) query = query.Where(a => userIds.Contains(a.EntityId!.Value));
        var events = await query
            .OrderBy(a => a.CreatedAt)
            .Select(a => new { UserId = a.EntityId!.Value, a.Action, a.Meta })
            .ToListAsync(ct);

        var roles = new Dictionary<Guid, HashSet<string>>();
        foreach (var item in events)
        {
            if (!roles.TryGetValue(item.UserId, out var set)) roles[item.UserId] = set = [];
            var role = AuditReader.Read<RoleMeta>(item.Meta).Role;
            if (item.Action == AuditActions.AssignRole) set.Add(role);
            else set.Remove(role);
        }
        return roles.ToDictionary(r => r.Key, r => (IReadOnlyList<string>)r.Value.Order().ToList());
    }

    public async Task<IReadOnlyList<string>> RolesOfAsync(Guid userId, CancellationToken ct) =>
        (await RolesAsync([userId], ct)).GetValueOrDefault(userId) ?? [];

    public async Task<IReadOnlyList<Guid>> UsersInRolesAsync(IReadOnlyCollection<string> roleCodes, CancellationToken ct) =>
        (await RolesAsync(null, ct)).Where(r => r.Value.Any(roleCodes.Contains)).Select(r => r.Key).ToList();

    public async Task EnsureActiveTeachersAsync(CancellationToken ct, params Guid?[] userIds)
    {
        var ids = userIds.OfType<Guid>().Distinct().ToList();
        if (ids.Count == 0) return;
        var active = await db.Users.Where(u => ids.Contains(u.Id) && u.Status == UserStatuses.Active).Select(u => u.Id).ToListAsync(ct);
        var roles = await RolesAsync(ids, ct);
        if (ids.Any(id => !active.Contains(id) || !roles.TryGetValue(id, out var codes) || !codes.Contains(RoleCodes.Teacher)))
            throw AppException.Invalid("INVALID_TEACHER", "Người dùng được chọn không phải giáo viên đang hoạt động");
    }
}
