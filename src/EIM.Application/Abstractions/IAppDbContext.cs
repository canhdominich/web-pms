using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EIM.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<AuthUser> Users { get; }
    DbSet<AuthRole> Roles { get; }
    DbSet<Student> Students { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<CurriculumProgram> Programs { get; }
    DbSet<Classroom> Classes { get; }
    DbSet<Session> Sessions { get; }
    DbSet<SessionFeedback> SessionFeedback { get; }
    DbSet<SessionScore> SessionScores { get; }
    DbSet<FeePlan> FeePlans { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<TrialLead> TrialLeads { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Notification> Notifications { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
