using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Enrollments;

public interface IEnrollmentService
{
    Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken ct);
    Task<IReadOnlyList<EnrollmentDto>> ListByStudentAsync(Guid studentId, CancellationToken ct);
    Task<EnrollmentDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<EnrollmentDto> ChangeStatusAsync(Guid id, ChangeEnrollmentStatusRequest request, CancellationToken ct);
    Task<EnrollmentDetailDto> TransferAsync(Guid id, TransferEnrollmentRequest request, CancellationToken ct);
}

internal sealed class EnrollmentService(IAppDbContext db, Enroller enroller, IAuditTrail audit, IClock clock) : IEnrollmentService
{
    public Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var enrollment = (await enroller.EnrollAsync(request.ClassId, [request.StudentId], request.StartDate, ct))[0];
            audit.Record(AuditActions.Enroll, AuditEntities.Enrollment, enrollment.Id,
                new { enrollment.StudentId, enrollment.ClassId, enrollment.StartDate });
            await db.SaveChangesAsync(ct);
            return (await db.ToDtosAsync([enrollment.Id], ct))[0];
        }, ct);

    public async Task<IReadOnlyList<EnrollmentDto>> ListByStudentAsync(Guid studentId, CancellationToken ct)
    {
        if (!await db.Students.AnyAsync(s => s.Id == studentId, ct)) throw AppException.NotFound("học sinh");
        return await db.Enrollments.AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.StartDate).ThenByDescending(e => e.CreatedAt)
            .Select(EnrollmentProjections.ToDto)
            .ToListAsync(ct);
    }

    public async Task<EnrollmentDetailDto> GetAsync(Guid id, CancellationToken ct)
    {
        var enrollment = await db.Enrollments.AsNoTracking().Where(e => e.Id == id).Select(EnrollmentProjections.ToDto).FirstOrDefaultAsync(ct)
                         ?? throw AppException.NotFound("ghi danh");
        var history = await db.AuditLogs.AsNoTracking()
            .Where(a => a.Entity == AuditEntities.Enrollment && a.EntityId == id)
            .OrderBy(a => a.CreatedAt)
            .Select(a => new EnrollmentHistoryDto(a.CreatedAt, a.Action, a.ActorUserId, a.Actor == null ? null : a.Actor.FullName, a.Meta))
            .ToListAsync(ct);
        return new EnrollmentDetailDto(enrollment, history);
    }

    public async Task<EnrollmentDto> ChangeStatusAsync(Guid id, ChangeEnrollmentStatusRequest request, CancellationToken ct)
    {
        var enrollment = await db.Enrollments.Include(e => e.Classroom).FirstOrDefaultAsync(e => e.Id == id, ct)
                         ?? throw AppException.NotFound("ghi danh");
        var target = Guard.OneOf(request.Status, EnrollmentStatuses.All, "Trạng thái");
        if (!EnrollmentStatuses.ManualTransitions.TryGetValue(enrollment.Status, out var allowed) || !allowed.Contains(target))
            throw AppException.Conflict("INVALID_TRANSITION", $"Không thể chuyển trạng thái từ {enrollment.Status} sang {target}");
        if (target == EnrollmentStatuses.Active && enrollment.Classroom.Status == ClassStatuses.Closed)
            throw AppException.Conflict("CLASS_CLOSED", "Lớp đã đóng, không thể kích hoạt lại ghi danh");

        var effective = request.EffectiveDate ?? clock.Today;
        var from = enrollment.Status;
        enrollment.Status = target;
        if (target is EnrollmentStatuses.Dropped or EnrollmentStatuses.Graduated) enrollment.EndDate = effective;

        audit.Record(AuditActions.StatusChange, AuditEntities.Enrollment, id, new
        {
            From = from,
            To = target,
            Reason = request.Reason.Trim(),
            EffectiveDate = effective
        });
        await db.SaveChangesAsync(ct);
        return (await db.ToDtosAsync([id], ct))[0];
    }

    public Task<EnrollmentDetailDto> TransferAsync(Guid id, TransferEnrollmentRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var source = await db.Enrollments.FirstOrDefaultAsync(e => e.Id == id, ct) ?? throw AppException.NotFound("ghi danh");
            if (!EnrollmentStatuses.Occupying.Contains(source.Status))
                throw AppException.Conflict("INVALID_TRANSITION", "Chỉ chuyển lớp cho ghi danh đang học hoặc bảo lưu");
            if (source.ClassId == request.TargetClassId)
                throw AppException.Invalid("SAME_CLASS", "Lớp chuyển đến phải khác lớp hiện tại");

            var transferDate = request.TransferDate ?? clock.Today;
            var lastSession = await db.Sessions.AsNoTracking()
                .Where(s => s.ClassId == source.ClassId && s.SessionDate < transferDate)
                .OrderByDescending(s => s.SessionDate)
                .Select(s => new { s.SessionDate, s.UnitNo, s.LessonNo })
                .FirstOrDefaultAsync(ct);

            var target = (await enroller.EnrollAsync(request.TargetClassId, [source.StudentId], transferDate, ct))[0];
            var fromStatus = source.Status;
            source.Status = EnrollmentStatuses.Transferred;
            source.EndDate = transferDate.AddDays(-1);

            var meta = new
            {
                FromStatus = fromStatus,
                FromClassId = source.ClassId,
                ToClassId = target.ClassId,
                FromEnrollmentId = source.Id,
                ToEnrollmentId = target.Id,
                TransferDate = transferDate,
                LastUnitNo = lastSession?.UnitNo,
                LastLessonNo = lastSession?.LessonNo,
                Reason = request.Reason.Trim()
            };
            audit.Record(AuditActions.Transfer, AuditEntities.Enrollment, source.Id, meta);
            audit.Record(AuditActions.Enroll, AuditEntities.Enrollment, target.Id, meta);
            await db.SaveChangesAsync(ct);
            return await GetAsync(target.Id, ct);
        }, ct);
}
