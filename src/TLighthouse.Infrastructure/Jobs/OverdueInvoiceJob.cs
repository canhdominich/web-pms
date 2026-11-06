using TLighthouse.Application.Features.Finance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TLighthouse.Infrastructure.Jobs;

public sealed class OverdueInvoiceJob(IServiceScopeFactory scopes, IOptions<JobOptions> options, ILogger<OverdueInvoiceJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.OverdueIntervalMinutes));
        do
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var processed = await scope.ServiceProvider.GetRequiredService<IOverdueInvoiceProcessor>().ProcessAsync(stoppingToken);
                if (processed > 0) logger.LogInformation("Marked {Count} invoices as overdue", processed);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Overdue invoice processing failed");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
