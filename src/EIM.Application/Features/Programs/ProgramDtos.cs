using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;

namespace EIM.Application.Features.Programs;

public sealed class ProgramQuery : PageQuery
{
    public string? Search { get; init; }
    public string? Level { get; init; }
}

public sealed record ProgramDto(
    Guid Id,
    string Code,
    string Name,
    string Level,
    int TotalUnits,
    int LessonsPerUnit,
    int SessionsPerWeek,
    Guid? FeePlanId,
    string? FeePlanName,
    int? FeePlanAmount,
    int ActiveClassCount,
    DateTime CreatedAt);

public sealed record LessonSlotDto(int Order, IReadOnlyList<int> Lessons);

public sealed record UnitDto(int UnitNo, IReadOnlyList<LessonSlotDto> Slots, string? AssessmentAfter);

public sealed record ProgramStructureDto(
    Guid ProgramId,
    int TotalUnits,
    int LessonsPerUnit,
    int SessionsPerWeek,
    int PlannedSessions,
    IReadOnlyList<UnitDto> Units);

public sealed record ProgramDetailDto(ProgramDto Program, ProgramStructureDto Structure);

public abstract record ProgramStructureRequest
{
    [Range(1, 50)]
    public int TotalUnits { get; init; }

    [Range(1, 20)]
    public int LessonsPerUnit { get; init; } = 7;
}

public sealed record CreateProgramRequest : ProgramStructureRequest
{
    [Required, MaxLength(50), RegularExpression("^[A-Za-z0-9_-]+$")]
    public string Code { get; init; } = "";

    [Required, MaxLength(255)]
    public string Name { get; init; } = "";

    [Required, MaxLength(100)]
    public string Level { get; init; } = "";

    [Range(1, 7)]
    public int SessionsPerWeek { get; init; } = 2;
}

public sealed record UpdateProgramRequest : ProgramStructureRequest
{
    [Required, MaxLength(255)]
    public string Name { get; init; } = "";

    [Required, MaxLength(100)]
    public string Level { get; init; } = "";

    [Range(1, 7)]
    public int SessionsPerWeek { get; init; } = 2;

    public Guid? FeePlanId { get; init; }
}

public sealed record UpdateUnitsRequest : ProgramStructureRequest;
