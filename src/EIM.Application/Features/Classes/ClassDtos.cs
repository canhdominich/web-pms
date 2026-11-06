using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;
using EIM.Application.Features.Enrollments;

namespace EIM.Application.Features.Classes;

public sealed class ClassQuery : PageQuery
{
    public Guid? ProgramId { get; init; }
    public string? Status { get; init; }
    public string? Search { get; init; }
}

public sealed record ClassDto(
    Guid Id,
    string Code,
    string Name,
    Guid ProgramId,
    string ProgramCode,
    string ProgramName,
    string? Room,
    int Capacity,
    int Enrolled,
    DateOnly StartDate,
    string Status,
    DateTime CreatedAt);

public sealed record RosterItemDto(
    Guid EnrollmentId,
    Guid StudentId,
    string FullName,
    string? Phone,
    string? GuardianName,
    string? GuardianPhone,
    string Status,
    DateOnly StartDate,
    DateOnly? EndDate);

public sealed record ScheduleDto(
    Guid ClassId,
    IReadOnlyList<DayOfWeek> Weekdays,
    int TotalSessions,
    int CompletedSessions,
    int LockedSessions,
    DateOnly? FirstSessionDate,
    DateOnly? LastSessionDate,
    DateOnly? NextSessionDate);

public sealed record ClassTeacherDto(
    Guid TeacherId,
    string FullName,
    string Email,
    string Assignment,
    int SessionCount,
    int UpcomingSessionCount,
    DateOnly FirstSessionDate,
    DateOnly LastSessionDate);

public sealed record ClassDetailDto(
    ClassDto Class,
    IReadOnlyList<RosterItemDto> Roster,
    IReadOnlyList<ClassTeacherDto> Teachers,
    ScheduleDto Schedule);

public sealed record GenerateSessionsRequest
{
    [Required, MinLength(1), MaxLength(7)]
    public IReadOnlyList<DayOfWeek> Weekdays { get; init; } = [];

    public Guid? MainTeacherId { get; init; }
    public DateOnly? FromDate { get; init; }
}

public sealed record RescheduleRequest
{
    [Required, MinLength(1), MaxLength(7)]
    public IReadOnlyList<DayOfWeek> Weekdays { get; init; } = [];

    public required DateOnly EffectiveDate { get; init; }
}

public sealed record CreateClassRequest
{
    [Required, MaxLength(50), RegularExpression("^[A-Za-z0-9_-]+$")]
    public string Code { get; init; } = "";

    [Required, MaxLength(255)]
    public string Name { get; init; } = "";

    public required Guid ProgramId { get; init; }

    [MaxLength(100)]
    public string? Room { get; init; }

    [Range(1, 100)]
    public int Capacity { get; init; } = 16;

    public required DateOnly StartDate { get; init; }

    public GenerateSessionsRequest? Schedule { get; init; }
}

public sealed record UpdateClassRequest
{
    [Required, MaxLength(255)]
    public string Name { get; init; } = "";

    [MaxLength(100)]
    public string? Room { get; init; }

    [Range(1, 100)]
    public int Capacity { get; init; }

    [Required, MaxLength(50)]
    public string Status { get; init; } = "";
}

public sealed record AddStudentRequest
{
    public required Guid StudentId { get; init; }
    public DateOnly? StartDate { get; init; }
}

public sealed record PromotionRequest
{
    public required Guid TargetClassId { get; init; }
    public DateOnly? StartDate { get; init; }
    public IReadOnlyList<Guid>? EnrollmentIds { get; init; }
    public bool CloseSourceClass { get; init; }
}

public sealed record PromotionResultDto(Guid SourceClassId, Guid TargetClassId, bool SourceClosed, IReadOnlyList<EnrollmentDto> Enrollments);

public sealed record AssignTeacherRequest
{
    public required Guid TeacherId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public bool ReplaceExisting { get; init; }
}

public sealed record ChangeMainTeacherRequest
{
    public required Guid TeacherId { get; init; }
    public required DateOnly EffectiveDate { get; init; }
}

public sealed record StaffChangeResultDto(Guid ClassId, int AffectedSessions, IReadOnlyList<ClassTeacherDto> Teachers);
