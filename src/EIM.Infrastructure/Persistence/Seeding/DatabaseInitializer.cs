using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EIM.Infrastructure.Persistence.Seeding;

internal sealed class DatabaseInitializer(
    AppDbContext db,
    IPasswordHasher hasher,
    RoleDirectory roles,
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
        await EnsureRolesAsync(ct);
        await EnsureRootAsync(ct);
        if (demo.Value.Enabled && demo.Value.SeedOnStartup && !await db.Programs.AnyAsync(ct))
        {
            await demoSeeder.SeedAsync(ct);
            logger.LogInformation("Demo data seeded");
        }
    }

    public async Task EnsureRolesAsync(CancellationToken ct)
    {
        var existing = await db.Roles.Select(r => r.Code).ToListAsync(ct);
        db.Roles.AddRange(RoleCodes.Names.Where(r => !existing.Contains(r.Key)).Select(r => new AuthRole { Code = r.Key, Name = r.Value }));
        await db.SaveChangesAsync(ct);
    }

    public async Task EnsureRootAsync(CancellationToken ct)
    {
        var email = seed.Value.RootEmail.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null)
        {
            user = new AuthUser { Email = email, FullName = seed.Value.RootFullName, PasswordHash = hasher.Hash(seed.Value.RootPassword) };
            db.Users.Add(user);
            logger.LogInformation("Root account {Email} created", email);
        }
        if (!(await roles.RolesOfAsync(user.Id, ct)).Contains(RoleCodes.Root)) roles.Assign(user.Id, RoleCodes.Root);
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
