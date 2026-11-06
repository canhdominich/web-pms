using TLighthouse.Domain.Constants;

namespace TLighthouse.Domain.Entities;

public class Classroom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid ProgramId { get; set; }
    public string? Room { get; set; }
    public int Capacity { get; set; } = 16;
    public DateOnly StartDate { get; set; }
    public string Status { get; set; } = ClassStatuses.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public CurriculumProgram Program { get; set; } = null!;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
