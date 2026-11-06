using System.ComponentModel.DataAnnotations;

namespace TLighthouse.Application.Features.Enrollments;

public sealed record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid ClassId,
    string ClassCode,
    string ClassName,
    string ProgramCode,
    string Status,
    DateOnly StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt);

public sealed record EnrollmentHistoryDto(DateTime At, string Action, Guid? ActorUserId, string? ActorName, string Meta);

public sealed record EnrollmentDetailDto(EnrollmentDto Enrollment, IReadOnlyList<EnrollmentHistoryDto> History);

public sealed record CreateEnrollmentRequest
{
    public required Guid StudentId { get; init; }
    public required Guid ClassId { get; init; }
    public DateOnly? StartDate { get; init; }
}

public sealed record ChangeEnrollmentStatusRequest
{
    [Required, MaxLength(50)]
    public string Status { get; init; } = "";

    [Required, MaxLength(1000)]
    public string Reason { get; init; } = "";

    public DateOnly? EffectiveDate { get; init; }
}

public sealed record TransferEnrollmentRequest
{
    public required Guid TargetClassId { get; init; }
    public DateOnly? TransferDate { get; init; }

    [Required, MaxLength(1000)]
    public string Reason { get; init; } = "";
}
