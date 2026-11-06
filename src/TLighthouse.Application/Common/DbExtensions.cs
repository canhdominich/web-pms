using TLighthouse.Application.Abstractions;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Common;

internal static class DbExtensions
{
    public static async Task<T> InTransactionAsync<T>(this IAppDbContext db, Func<Task<T>> action, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is not null) return await action();
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var result = await action();
        await transaction.CommitAsync(ct);
        return result;
    }

    public static async Task<Classroom?> LockClassAsync(this IAppDbContext db, Guid id, CancellationToken ct) =>
        (await db.Classes.FromSqlInterpolated($"SELECT * FROM classes WHERE id = {id} FOR UPDATE").ToListAsync(ct)).FirstOrDefault();

    public static async Task<Invoice?> LockInvoiceAsync(this IAppDbContext db, Guid id, CancellationToken ct) =>
        (await db.Invoices.FromSqlInterpolated($"SELECT * FROM finance_invoices WHERE id = {id} FOR UPDATE").ToListAsync(ct)).FirstOrDefault();

    public static IQueryable<Enrollment> Occupying(this IQueryable<Enrollment> query, Guid classId) =>
        query.Where(e => e.ClassId == classId && EnrollmentStatuses.Occupying.Contains(e.Status));

    public static IQueryable<Enrollment> AttendingOn(this IQueryable<Enrollment> query, Guid classId, DateOnly date) =>
        query.Where(e => e.ClassId == classId
                         && e.Status != EnrollmentStatuses.Paused
                         && e.StartDate <= date
                         && (e.EndDate == null || e.EndDate >= date));

    public static IQueryable<Session> TaughtBy(this IQueryable<Session> query, Guid teacherId) =>
        query.Where(s => s.CoverTeacherId == teacherId || (s.CoverTeacherId == null && s.MainTeacherId == teacherId));

    public static IQueryable<Session> Unlocked(this IQueryable<Session> query) =>
        query.Where(s => !s.Feedbacks.Any() && !s.Scores.Any());
}
