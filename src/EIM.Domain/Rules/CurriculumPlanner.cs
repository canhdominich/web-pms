using EIM.Domain.Constants;

namespace EIM.Domain.Rules;

public sealed record LessonSlot(int Order, IReadOnlyList<int> Lessons);

public sealed record UnitPlan(int UnitNo, IReadOnlyList<LessonSlot> Slots, string? AssessmentAfter);

public sealed record PlannedSession(int UnitNo, int LessonNo, string SessionType);

public static class CurriculumPlanner
{
    public const int FinalUnit = 12;
    public const int MidtermUnit = 6;
    public const int QuizEvery = 2;

    private static readonly int[][] StandardSevenLessonPattern = [[1, 2], [3], [4, 5], [6, 7]];

    public static IReadOnlyList<LessonSlot> LessonSlots(int lessonsPerUnit)
    {
        var groups = lessonsPerUnit == 7
            ? StandardSevenLessonPattern
            : Enumerable.Range(1, lessonsPerUnit).Select(l => new[] { l }).ToArray();
        return groups.Select((g, i) => new LessonSlot(i + 1, g)).ToList();
    }

    public static string? AssessmentAfterUnit(int unitNo, int totalUnits)
    {
        if (unitNo == totalUnits && totalUnits >= FinalUnit) return SessionTypes.Final;
        if (unitNo == MidtermUnit) return SessionTypes.Midterm;
        return unitNo % QuizEvery == 0 ? SessionTypes.Test : null;
    }

    public static IReadOnlyList<UnitPlan> Units(int totalUnits, int lessonsPerUnit)
    {
        var slots = LessonSlots(lessonsPerUnit);
        return Enumerable.Range(1, totalUnits)
            .Select(u => new UnitPlan(u, slots, AssessmentAfterUnit(u, totalUnits)))
            .ToList();
    }

    public static IReadOnlyList<PlannedSession> Sessions(int totalUnits, int lessonsPerUnit)
    {
        var result = new List<PlannedSession>();
        foreach (var unit in Units(totalUnits, lessonsPerUnit))
        {
            result.AddRange(unit.Slots.Select(s => new PlannedSession(unit.UnitNo, s.Lessons[0], SessionTypes.Normal)));
            if (unit.AssessmentAfter is not null)
                result.Add(new PlannedSession(unit.UnitNo, lessonsPerUnit, unit.AssessmentAfter));
        }
        return result;
    }
}
