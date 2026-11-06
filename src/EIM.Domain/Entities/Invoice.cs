using EIM.Domain.Constants;

namespace EIM.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnrollmentId { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; } = InvoiceStatuses.Draft;
    public DateOnly DueDate { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Enrollment Enrollment { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
