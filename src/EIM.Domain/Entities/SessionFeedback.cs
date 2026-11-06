namespace EIM.Domain.Entities;

public class SessionFeedback
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid StudentId { get; set; }
    public string? Attendance { get; set; }
    public string? Homework { get; set; }
    public string? Participation { get; set; }
    public string? Behavior { get; set; }
    public string? CommentText { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Session Session { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public AuthUser Teacher { get; set; } = null!;
}
