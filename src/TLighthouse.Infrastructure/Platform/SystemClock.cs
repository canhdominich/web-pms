using TLighthouse.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace TLighthouse.Infrastructure.Platform;

public sealed class SystemClock(IOptions<ClockOptions> options) : IClock
{
    private readonly TimeZoneInfo _zone = Resolve(options.Value.TimeZone);

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly Today => LocalDate(DateTime.UtcNow);

    public DateOnly LocalDate(DateTime utc) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), _zone));

    private static TimeZoneInfo Resolve(string id) =>
        TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone) ? zone : TimeZoneInfo.Utc;
}
