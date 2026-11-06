using System.Text.Json;
using TLighthouse.Application.Abstractions;
using TLighthouse.Domain.Entities;

namespace TLighthouse.Application.Common;

internal sealed class AuditTrail(IAppDbContext db, ICurrentUser currentUser, IClock clock) : IAuditTrail
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Record(string action, string entity, Guid? entityId, object? meta = null) =>
        db.AuditLogs.Add(new AuditLog
        {
            ActorUserId = currentUser.UserId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            Meta = JsonSerializer.Serialize(meta ?? new { }, JsonOptions),
            CreatedAt = clock.UtcNow
        });
}
