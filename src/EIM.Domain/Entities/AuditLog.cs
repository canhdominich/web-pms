namespace EIM.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = null!;
    public string Entity { get; set; } = null!;
    public Guid? EntityId { get; set; }
    public string Meta { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AuthUser? Actor { get; set; }
}
