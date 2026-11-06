using TLighthouse.Application.Abstractions;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TLighthouse.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    private static readonly string[] TimestampProperties = ["CreatedAt", "UpdatedAt"];

    public DbSet<AuthUser> Users => Set<AuthUser>();
    public DbSet<AuthRole> Roles => Set<AuthRole>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<CurriculumProgram> Programs => Set<CurriculumProgram>();
    public DbSet<Classroom> Classes => Set<Classroom>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionFeedback> SessionFeedback => Set<SessionFeedback>();
    public DbSet<SessionScore> SessionScores => Set<SessionScore>();
    public DbSet<FeePlan> FeePlans => Set<FeePlan>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TrialLead> TrialLeads => Set<TrialLead>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>().HaveColumnType("text");
        builder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        builder.Properties<DateTime?>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasCharSet("utf8mb4");
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetProperties())
                     .Where(p => TimestampProperties.Contains(p.Name) && p.ClrType == typeof(DateTime)))
            property.SetDefaultValueSql("CURRENT_TIMESTAMP(6)");
    }

    private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
}
