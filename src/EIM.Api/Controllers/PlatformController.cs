using EIM.Api.Infrastructure;
using EIM.Application.Common;
using EIM.Application.Features.Platform;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api/audit-logs"), Authorize(Roles = RoleSets.AuditViewers)]
public sealed class AuditLogsController(IAuditLogService logs) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<AuditLogDto>> List([FromQuery] AuditLogQuery query, CancellationToken ct) => logs.ListAsync(query, ct);
}

[Route("api/notifications"), Authorize]
public sealed class NotificationsController(INotificationService notifications) : ApiControllerBase
{
    [HttpGet("me")]
    public Task<PagedResult<NotificationDto>> Mine([FromQuery] NotificationQuery query, CancellationToken ct) => notifications.ListMineAsync(query, ct);

    [HttpGet("me/unread-count")]
    public Task<UnreadCountDto> UnreadCount(CancellationToken ct) => notifications.CountUnreadAsync(ct);

    [HttpPatch("{id:guid}")]
    public Task<NotificationDto> Mark(Guid id, MarkNotificationRequest request, CancellationToken ct) => notifications.MarkAsync(id, request, ct);

    [HttpPost("me/read-all")]
    public Task<UnreadCountDto> MarkAllRead(CancellationToken ct) => notifications.MarkAllReadAsync(ct);

    [HttpPost, Authorize(Roles = RoleSets.Staff)]
    public Task<SendNotificationResultDto> Send(SendNotificationRequest request, CancellationToken ct) => notifications.SendAsync(request, ct);
}

[Route("api/system/demo"), Authorize(Roles = RoleSets.Root)]
public sealed class DemoController(IDemoService demo) : ApiControllerBase
{
    [HttpPost("reset")]
    public Task<DemoResetResultDto> Reset(CancellationToken ct) => demo.ResetAsync(ct);
}
