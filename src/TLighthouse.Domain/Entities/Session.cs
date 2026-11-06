using TLighthouse.Domain.Constants;

namespace TLighthouse.Domain.Entities;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public DateOnly SessionDate { get; set; }
    public int UnitNo { get; set; }
    public int LessonNo { get; set; }
    public string SessionType { get; set; } = SessionTypes.Normal;
    public Guid? MainTeacherId { get; set; }
    public Guid? CoverTeacherId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Classroom Classroom { get; set; } = null!;
    public AuthUser? MainTeacher { get; set; }
    public AuthUser? CoverTeacher { get; set; }
    public ICollection<SessionFeedback> Feedbacks { get; set; } = new List<SessionFeedback>();
    public ICollection<SessionScore> Scores { get; set; } = new List<SessionScore>();

    public Guid? EffectiveTeacherId => CoverTeacherId ?? MainTeacherId;
}
