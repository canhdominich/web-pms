using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Application.Features.Platform;
using EIM.Domain.Constants;
using EIM.Infrastructure.Persistence;
using EIM.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EIM.Infrastructure.Platform;

internal sealed class DemoService(
    AppDbContext db,
    RoleDirectory roles,
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
        var rootIds = await roles.UsersInRolesAsync([RoleCodes.Root], ct);

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
        await db.AuditLogs
            .Where(a => !(a.Entity == AuditEntities.User
                          && a.EntityId != null
                          && rootIds.Contains(a.EntityId.Value)
                          && (a.Action == AuditActions.AssignRole || a.Action == AuditActions.RevokeRole)))
            .ExecuteDeleteAsync(ct);
        await db.Users.Where(u => !rootIds.Contains(u.Id)).ExecuteDeleteAsync(ct);

        db.ChangeTracker.Clear();
        await initializer.EnsureRolesAsync(ct);
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
