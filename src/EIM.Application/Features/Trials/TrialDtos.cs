using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;
using EIM.Application.Features.Enrollments;
using EIM.Application.Features.Students;

namespace EIM.Application.Features.Trials;

public sealed class TrialLeadQuery : PageQuery
{
    public string? Search { get; init; }
    public string? Status { get; init; }
    public string? Source { get; init; }
}

public sealed record TrialLeadDto(
    Guid Id,
    string FullName,
    string Phone,
    string? Email,
    string? Source,
    string Status,
    string? Note,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt);

public sealed record TrialScheduleDto(
    Guid Id,
    Guid LeadId,
    Guid SessionId,
    Guid ClassId,
    string ClassCode,
    DateOnly SessionDate,
    int UnitNo,
    int LessonNo,
    string Status,
    string? Note,
    DateTime CreatedAt);

public sealed record TrialConversionDto(Guid Id, Guid LeadId, Guid StudentId, Guid EnrollmentId, Guid ClassId, Guid? ConvertedBy, DateTime CreatedAt);

public sealed record TrialLeadDetailDto(TrialLeadDto Lead, IReadOnlyList<TrialScheduleDto> Schedules, TrialConversionDto? Conversion);

public sealed record SaveTrialLeadRequest
{
    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [Required, MaxLength(20), RegularExpression(@"^[0-9+\-\s().]{6,20}$")]
    public string Phone { get; init; } = "";

    [EmailAddress, MaxLength(255)]
    public string? Email { get; init; }

    [MaxLength(100)]
    public string? Source { get; init; }

    [MaxLength(2000)]
    public string? Note { get; init; }
}

public sealed record ChangeTrialStatusRequest
{
    [Required, MaxLength(20)]
    public string Status { get; init; } = "";

    [MaxLength(2000)]
    public string? Note { get; init; }
}

public sealed record ScheduleTrialRequest
{
    public required Guid SessionId { get; init; }

    [MaxLength(2000)]
    public string? Note { get; init; }
}

public sealed record TrialResultRequest
{
    [Required, MaxLength(20)]
    public string Status { get; init; } = "";

    [MaxLength(2000)]
    public string? Note { get; init; }
}

public sealed record ConvertTrialRequest : StudentProfileRequest
{
    public Guid? ExistingStudentId { get; init; }
    public required Guid ClassId { get; init; }
    public DateOnly? StartDate { get; init; }
}

public sealed record ConvertTrialResultDto(TrialConversionDto Conversion, StudentDto Student, EnrollmentDto Enrollment);
