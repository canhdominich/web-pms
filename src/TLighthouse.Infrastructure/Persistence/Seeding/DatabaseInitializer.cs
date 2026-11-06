using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TLighthouse.Infrastructure.Persistence.Seeding;

internal sealed class DatabaseInitializer(
    AppDbContext db,
    IPasswordHasher hasher,
    DemoDataSeeder demoSeeder,
    IOptions<DatabaseOptions> database,
    IOptions<SeedOptions> seed,
    IOptions<DemoOptions> demo,
    ILogger<DatabaseInitializer> logger)
{
    private const int MaxAttempts = 10;

    public async Task InitializeAsync(CancellationToken ct)
    {
        if (database.Value.CreateSchemaOnStartup) await CreateSchemaAsync(ct);
        await EnsureRootAsync(ct);
        if (demo.Value.Enabled && demo.Value.SeedOnStartup && !await db.Programs.AnyAsync(ct))
        {
            await demoSeeder.SeedAsync(ct);
            logger.LogInformation("Demo data seeded");
        }
    }

    public async Task EnsureRootAsync(CancellationToken ct)
    {
        var email = seed.Value.RootEmail.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null)
        {
            user = new AuthUser { Email = email, FullName = seed.Value.RootFullName, PasswordHash = hasher.Hash(seed.Value.RootPassword), Role = RoleCodes.Root };
            db.Users.Add(user);
            logger.LogInformation("Root account {Email} created", email);
        }
        else if (user.Role != RoleCodes.Root) user.Role = RoleCodes.Root;
        await db.SaveChangesAsync(ct);
    }

    private async Task CreateSchemaAsync(CancellationToken ct)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                if (await db.Database.EnsureCreatedAsync(ct)) logger.LogInformation("Database schema created");
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts && ex is not OperationCanceledException)
            {
                logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Message}", attempt, MaxAttempts, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }
}
