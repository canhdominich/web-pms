using EIM.Domain.Constants;

namespace EIM.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public string Status { get; set; } = EnrollmentStatuses.Active;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Student Student { get; set; } = null!;
    public Classroom Classroom { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
