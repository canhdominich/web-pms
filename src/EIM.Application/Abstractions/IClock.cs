namespace EIM.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
    DateOnly LocalDate(DateTime utc);
}
