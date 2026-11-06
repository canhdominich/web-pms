namespace EIM.Domain.Entities;

public class SessionScore
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid StudentId { get; set; }
    public string ScoreType { get; set; } = null!;
    public int? Listening { get; set; }
    public int? Reading { get; set; }
    public int? Writing { get; set; }
    public int? Speaking { get; set; }
    public int Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Session Session { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
