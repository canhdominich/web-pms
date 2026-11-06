namespace TLighthouse.Domain.Rules;

public static class WeeklyCalendar
{
    public static IReadOnlyList<DateOnly> Dates(DateOnly from, IReadOnlyCollection<DayOfWeek> weekdays, int count)
    {
        var days = weekdays.ToHashSet();
        var result = new List<DateOnly>(count);
        for (var date = from; result.Count < count; date = date.AddDays(1))
            if (days.Contains(date.DayOfWeek)) result.Add(date);
        return result;
    }
}
