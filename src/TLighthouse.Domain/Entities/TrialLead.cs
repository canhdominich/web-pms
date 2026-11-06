using TLighthouse.Domain.Constants;

namespace TLighthouse.Domain.Entities;

public class TrialLead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = TrialLeadStatuses.New;
    public string? Note { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AuthUser Creator { get; set; } = null!;
}
