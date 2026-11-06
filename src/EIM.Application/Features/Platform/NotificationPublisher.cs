using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Platform;

internal sealed class NotificationPublisher(IAppDbContext db, RoleDirectory roles, IClock clock)
{
    public async Task<int> PublishAsync(IReadOnlyCollection<Guid> userIds, IReadOnlyCollection<string> roleCodes, string title, string body, CancellationToken ct)
    {
        var targets = userIds.Concat(roleCodes.Count > 0 ? await roles.UsersInRolesAsync(roleCodes, ct) : []).Distinct().ToList();
        var recipients = await db.Users
            .Where(u => u.Status == UserStatuses.Active && targets.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(ct);
        db.Notifications.AddRange(recipients.Select(id => new Notification
        {
            UserId = id,
            Title = title,
            Body = body,
            CreatedAt = clock.UtcNow
        }));
        return recipients.Count;
    }

    public Task<int> PublishToRoleAsync(string roleCode, string title, string body, CancellationToken ct) =>
        PublishAsync([], [roleCode], title, body, ct);
}
