using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;

namespace EIM.Application.Features.Students;

public sealed class StudentQuery : PageQuery
{
    public string? Search { get; init; }
}

public sealed record StudentDto(
    Guid Id,
    string FullName,
    DateOnly? Dob,
    string? Gender,
    string? Phone,
    string? Email,
    string? GuardianName,
    string? GuardianPhone,
    string? Address,
    DateTime CreatedAt);

public record StudentProfileRequest
{
    public DateOnly? Dob { get; init; }

    [MaxLength(20)]
    public string? Gender { get; init; }

    [MaxLength(255)]
    public string? GuardianName { get; init; }

    [MaxLength(20), RegularExpression(@"^[0-9+\-\s().]{6,20}$")]
    public string? GuardianPhone { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }
}

public sealed record SaveStudentRequest : StudentProfileRequest
{
    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [MaxLength(20), RegularExpression(@"^[0-9+\-\s().]{6,20}$")]
    public string? Phone { get; init; }

    [EmailAddress, MaxLength(255)]
    public string? Email { get; init; }
}
