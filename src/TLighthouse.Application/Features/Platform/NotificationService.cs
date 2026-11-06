using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Platform;

public interface INotificationService
{
    Task<PagedResult<NotificationDto>> ListMineAsync(NotificationQuery query, CancellationToken ct);
    Task<UnreadCountDto> CountUnreadAsync(CancellationToken ct);
    Task<NotificationDto> MarkAsync(Guid id, MarkNotificationRequest request, CancellationToken ct);
    Task<UnreadCountDto> MarkAllReadAsync(CancellationToken ct);
    Task<SendNotificationResultDto> SendAsync(SendNotificationRequest request, CancellationToken ct);
}

internal sealed class NotificationService(
    IAppDbContext db,
    ICurrentUser currentUser,
    NotificationPublisher publisher,
    IAuditTrail audit) : INotificationService
{
    public Task<PagedResult<NotificationDto>> ListMineAsync(NotificationQuery query, CancellationToken ct)
    {
        var userId = currentUser.RequiredId;
        var notifications = db.Notifications.AsNoTracking().Where(n => n.UserId == userId);
        if (query.IsRead is { } isRead) notifications = notifications.Where(n => n.IsRead == isRead);
        return notifications.OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Body, n.IsRead, n.CreatedAt))
            .ToPagedAsync(query, ct);
    }

    public async Task<UnreadCountDto> CountUnreadAsync(CancellationToken ct)
    {
        var userId = currentUser.RequiredId;
        return new UnreadCountDto(await db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct));
    }

    public async Task<NotificationDto> MarkAsync(Guid id, MarkNotificationRequest request, CancellationToken ct)
    {
        var userId = currentUser.RequiredId;
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct)
                           ?? throw AppException.NotFound("thông báo");
        notification.IsRead = request.IsRead;
        await db.SaveChangesAsync(ct);
        return new NotificationDto(notification.Id, notification.Title, notification.Body, notification.IsRead, notification.CreatedAt);
    }

    public async Task<UnreadCountDto> MarkAllReadAsync(CancellationToken ct)
    {
        var userId = currentUser.RequiredId;
        await db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
        return new UnreadCountDto(0);
    }

    public async Task<SendNotificationResultDto> SendAsync(SendNotificationRequest request, CancellationToken ct)
    {
        var roles = request.RoleCodes.Select(r => r.Trim().ToUpperInvariant()).Distinct().ToList();
        if (roles.Any(r => !RoleCodes.Names.ContainsKey(r)))
            throw AppException.Invalid("INVALID_ROLE", "Có vai trò không tồn tại");
        var userIds = request.UserIds.Distinct().ToList();
        if (userIds.Count == 0 && roles.Count == 0)
            throw AppException.Invalid("RECIPIENT_REQUIRED", "Cần chọn người nhận hoặc vai trò nhận thông báo");

        var count = await publisher.PublishAsync(userIds, roles, request.Title.Trim(), request.Body.Trim(), ct);
        if (count == 0)
            throw AppException.Invalid("RECIPIENT_NOT_FOUND", "Không có người nhận hợp lệ");
        audit.Record(AuditActions.Send, AuditEntities.Notification, null, new { UserIds = userIds, Roles = roles, Title = request.Title.Trim(), Recipients = count });
        await db.SaveChangesAsync(ct);
        return new SendNotificationResultDto(count);
    }
}
