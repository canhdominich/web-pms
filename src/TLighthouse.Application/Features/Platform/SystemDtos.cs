using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TLighthouse.Application.Common;

namespace TLighthouse.Application.Features.Platform;

public sealed class AuditLogQuery : PageQuery
{
    public Guid? ActorUserId { get; init; }
    public string? Entity { get; init; }
    public string? Action { get; init; }
    public Guid? EntityId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}

public sealed record AuditLogDto(
    Guid Id,
    Guid? ActorUserId,
    string? ActorName,
    string Action,
    string Entity,
    Guid? EntityId,
    JsonElement Meta,
    DateTime CreatedAt);

public sealed class NotificationQuery : PageQuery
{
    public bool? IsRead { get; init; }
}

public sealed record NotificationDto(Guid Id, string Title, string Body, bool IsRead, DateTime CreatedAt);

public sealed record UnreadCountDto(int Unread);

public sealed record MarkNotificationRequest
{
    public bool IsRead { get; init; } = true;
}

public sealed record SendNotificationRequest
{
    public IReadOnlyList<Guid> UserIds { get; init; } = [];
    public IReadOnlyList<string> RoleCodes { get; init; } = [];

    [Required, MaxLength(255)]
    public string Title { get; init; } = "";

    [Required, MaxLength(4000)]
    public string Body { get; init; } = "";
}

public sealed record SendNotificationResultDto(int Recipients);

public sealed record DemoResetResultDto(DateTime ResetAt, IReadOnlyDictionary<string, int> Counts);
