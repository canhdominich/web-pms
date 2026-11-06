namespace EIM.Application.Features.Platform;

public interface IDemoService
{
    Task<DemoResetResultDto> ResetAsync(CancellationToken ct);
}
