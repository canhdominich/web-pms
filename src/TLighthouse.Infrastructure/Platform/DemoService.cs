using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Platform;
using TLighthouse.Domain.Constants;
using TLighthouse.Infrastructure.Persistence;
using TLighthouse.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TLighthouse.Infrastructure.Platform;

internal sealed class DemoService(
    AppDbContext db,
    DatabaseInitializer initializer,
    DemoDataSeeder seeder,
    IAuditTrail audit,
    IOptions<DemoOptions> options,
    IClock clock) : IDemoService
{
    public async Task<DemoResetResultDto> ResetAsync(CancellationToken ct)
    {
        if (!options.Value.Enabled)
            throw AppException.Forbidden("Chức năng reset chỉ khả dụng trong môi trường demo");

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var rootIds = await db.Users.Where(u => u.Role == RoleCodes.Root).Select(u => u.Id).ToListAsync(ct);

        await db.Payments.ExecuteDeleteAsync(ct);
        await db.SessionScores.ExecuteDeleteAsync(ct);
        await db.SessionFeedback.ExecuteDeleteAsync(ct);
        await db.Invoices.ExecuteDeleteAsync(ct);
        await db.TrialLeads.ExecuteDeleteAsync(ct);
        await db.Enrollments.ExecuteDeleteAsync(ct);
        await db.Sessions.ExecuteDeleteAsync(ct);
        await db.Classes.ExecuteDeleteAsync(ct);
        await db.Programs.ExecuteUpdateAsync(s => s.SetProperty(p => p.FeePlanId, (Guid?)null), ct);
        await db.FeePlans.ExecuteDeleteAsync(ct);
        await db.Programs.ExecuteDeleteAsync(ct);
        await db.Students.ExecuteDeleteAsync(ct);
        await db.Notifications.ExecuteDeleteAsync(ct);
        await db.AuditLogs.ExecuteDeleteAsync(ct);
        await db.Users.Where(u => !rootIds.Contains(u.Id)).ExecuteDeleteAsync(ct);

        db.ChangeTracker.Clear();
        await initializer.EnsureRootAsync(ct);
        await seeder.SeedAsync(ct);

        var counts = new Dictionary<string, int>
        {
            ["users"] = await db.Users.CountAsync(ct),
            ["programs"] = await db.Programs.CountAsync(ct),
            ["classes"] = await db.Classes.CountAsync(ct),
            ["sessions"] = await db.Sessions.CountAsync(ct),
            ["students"] = await db.Students.CountAsync(ct),
            ["enrollments"] = await db.Enrollments.CountAsync(ct),
            ["invoices"] = await db.Invoices.CountAsync(ct),
            ["trialLeads"] = await db.TrialLeads.CountAsync(ct)
        };
        audit.Record(AuditActions.Reset, AuditEntities.System, null, new { Counts = counts });
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new DemoResetResultDto(clock.UtcNow, counts);
    }
}
