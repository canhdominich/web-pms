namespace TLighthouse.Domain.Entities;

public class FeePlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProgramId { get; set; }
    public string Name { get; set; } = null!;
    public int Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public int SessionsPerWeek { get; set; } = 2;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public CurriculumProgram Program { get; set; } = null!;
}
