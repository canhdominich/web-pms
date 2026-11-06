using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Features.Platform;
using TLighthouse.Infrastructure.Excel;
using TLighthouse.Infrastructure.Jobs;
using TLighthouse.Infrastructure.Persistence;
using TLighthouse.Infrastructure.Persistence.Seeding;
using TLighthouse.Infrastructure.Platform;
using TLighthouse.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TLighthouse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(DatabaseOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<SeedOptions>().Bind(configuration.GetSection(SeedOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<DemoOptions>().Bind(configuration.GetSection(DemoOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<JobOptions>().Bind(configuration.GetSection(JobOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<ClockOptions>().Bind(configuration.GetSection(ClockOptions.Section));

        var database = configuration.GetSection(DatabaseOptions.Section).Get<DatabaseOptions>() ?? new DatabaseOptions();
        services.AddDbContext<AppDbContext>(options => options.UseTLighthouseMySql(database.ConnectionString, database.ServerVersion));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IExcelService, ExcelService>();
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<DemoDataSeeder>();
        services.AddScoped<IDemoService, DemoService>();
        services.AddHostedService<OverdueInvoiceJob>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync(ct);
    }
}
