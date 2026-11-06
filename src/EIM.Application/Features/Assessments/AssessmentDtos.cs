using System.ComponentModel.DataAnnotations;
using EIM.Application.Features.Sessions;

namespace EIM.Application.Features.Assessments;

public sealed record FeedbackDto(
    Guid Id,
    Guid SessionId,
    Guid StudentId,
    string? Attendance,
    string? Homework,
    string? Participation,
    string? Behavior,
    string? CommentText,
    Guid TeacherId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record ScoreDto(
    Guid Id,
    Guid SessionId,
    Guid StudentId,
    string ScoreType,
    int? Listening,
    int? Reading,
    int? Writing,
    int? Speaking,
    int Total,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record FeedbackFormRowDto(Guid StudentId, string StudentName, FeedbackDto? Feedback, ScoreDto? Score);

public sealed record FeedbackFormDto(SessionDto Session, bool ScoreRequired, bool Editable, IReadOnlyList<FeedbackFormRowDto> Rows);

public sealed record FeedbackItem
{
    public required Guid StudentId { get; init; }

    [Required, MaxLength(20)]
    public string Attendance { get; init; } = "";

    [MaxLength(20)]
    public string? Homework { get; init; }

    [MaxLength(1000)]
    public string? Participation { get; init; }

    [MaxLength(1000)]
    public string? Behavior { get; init; }

    [MaxLength(4000)]
    public string? CommentText { get; init; }
}

public sealed record SaveFeedbackRequest
{
    [Required, MinLength(1), MaxLength(100)]
    public IReadOnlyList<FeedbackItem> Items { get; init; } = [];
}

public sealed record ScoreItem
{
    public required Guid StudentId { get; init; }

    [Range(0, 100)]
    public int? Listening { get; init; }

    [Range(0, 100)]
    public int? Reading { get; init; }

    [Range(0, 100)]
    public int? Writing { get; init; }

    [Range(0, 100)]
    public int? Speaking { get; init; }

    [Range(0, 100)]
    public int? Total { get; init; }
}

public sealed record SaveScoresRequest
{
    [Required, MinLength(1), MaxLength(100)]
    public IReadOnlyList<ScoreItem> Items { get; init; } = [];
}

public sealed class FeedbackExportQuery
{
    public Guid? ClassId { get; init; }
    public Guid? SessionId { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}

public sealed record StudentAssessmentDto(
    Guid SessionId,
    Guid ClassId,
    string ClassCode,
    DateOnly SessionDate,
    int UnitNo,
    int LessonNo,
    string SessionType,
    FeedbackDto? Feedback,
    ScoreDto? Score);

public sealed record AttendanceSummaryDto(
    Guid EnrollmentId,
    Guid StudentId,
    Guid ClassId,
    DateOnly From,
    DateOnly To,
    int TotalSessions,
    int Recorded,
    int Present,
    int Late,
    int Absent,
    int Unrecorded,
    double AttendanceRate);
