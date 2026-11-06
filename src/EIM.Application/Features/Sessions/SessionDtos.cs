using System.ComponentModel.DataAnnotations;

namespace EIM.Application.Features.Sessions;

public sealed class SessionQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public string? SessionType { get; init; }
}

public sealed record SessionDto(
    Guid Id,
    Guid ClassId,
    string ClassCode,
    string ClassName,
    string? Room,
    DateOnly SessionDate,
    int UnitNo,
    int LessonNo,
    string SessionType,
    Guid? MainTeacherId,
    string? MainTeacherName,
    Guid? CoverTeacherId,
    string? CoverTeacherName,
    int FeedbackCount,
    int ScoreCount);

public sealed record SessionStudentDto(Guid StudentId, string FullName, Guid EnrollmentId, string EnrollmentStatus);

public sealed record SessionDetailDto(SessionDto Session, IReadOnlyList<SessionStudentDto> Roster, int TrialGuests);

public sealed record UpdateSessionRequest
{
    public required DateOnly SessionDate { get; init; }

    [Range(1, 50)]
    public int UnitNo { get; init; }

    [Range(1, 20)]
    public int LessonNo { get; init; }

    [Required, MaxLength(20)]
    public string SessionType { get; init; } = "";

    public Guid? MainTeacherId { get; init; }
    public Guid? CoverTeacherId { get; init; }
}
