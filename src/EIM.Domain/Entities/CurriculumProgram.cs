namespace EIM.Domain.Entities;

public class CurriculumProgram
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Level { get; set; } = null!;
    public int TotalUnits { get; set; }
    public int LessonsPerUnit { get; set; } = 7;
    public int SessionsPerWeek { get; set; } = 2;
    public Guid? FeePlanId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<FeePlan> FeePlans { get; set; } = new List<FeePlan>();
    public ICollection<Classroom> Classes { get; set; } = new List<Classroom>();
}
