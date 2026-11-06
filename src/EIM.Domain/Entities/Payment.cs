namespace EIM.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public int Amount { get; set; }
    public string Method { get; set; } = null!;
    public DateTime PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Invoice Invoice { get; set; } = null!;
}
