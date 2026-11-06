using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Classes;

public interface IClassStaffService
{
    Task<IReadOnlyList<ClassTeacherDto>> ListAsync(Guid classId, CancellationToken ct);
    Task<StaffChangeResultDto> AssignAsync(Guid classId, AssignTeacherRequest request, CancellationToken ct);
    Task<StaffChangeResultDto> UnassignAsync(Guid classId, Guid teacherId, DateOnly? fromDate, CancellationToken ct);
    Task<StaffChangeResultDto> ChangeMainAsync(Guid classId, ChangeMainTeacherRequest request, CancellationToken ct);
}

internal sealed class ClassStaffService(IAppDbContext db, RoleDirectory roles, IAuditTrail audit, IClock clock) : IClassStaffService
{
    public const string Main = "MAIN";
    public const string Cover = "COVER";

    public async Task<IReadOnlyList<ClassTeacherDto>> ListAsync(Guid classId, CancellationToken ct)
    {
        if (!await db.Classes.AnyAsync(c => c.Id == classId, ct)) throw AppException.NotFound("lớp học");
        var slots = await db.Sessions.AsNoTracking()
            .Where(s => s.ClassId == classId && (s.MainTeacherId != null || s.CoverTeacherId != null))
            .Select(s => new { s.SessionDate, s.MainTeacherId, s.CoverTeacherId })
            .ToListAsync(ct);

        var assignments = slots
            .Where(s => s.MainTeacherId != null).Select(s => (TeacherId: s.MainTeacherId!.Value, Kind: Main, s.SessionDate))
            .Concat(slots.Where(s => s.CoverTeacherId != null).Select(s => (TeacherId: s.CoverTeacherId!.Value, Kind: Cover, s.SessionDate)))
            .ToList();
        var ids = assignments.Select(a => a.TeacherId).Distinct().ToList();
        var users = await db.Users.AsNoTracking().Where(u => ids.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);
        var today = clock.Today;

        return assignments
            .GroupBy(a => (a.TeacherId, a.Kind))
            .Select(g => new ClassTeacherDto(
                g.Key.TeacherId,
                users[g.Key.TeacherId].FullName,
                users[g.Key.TeacherId].Email,
                g.Key.Kind,
                g.Count(),
                g.Count(a => a.SessionDate >= today),
                g.Min(a => a.SessionDate),
                g.Max(a => a.SessionDate)))
            .OrderBy(t => t.Assignment == Main ? 0 : 1).ThenBy(t => t.FirstSessionDate)
            .ToList();
    }

    public async Task<StaffChangeResultDto> AssignAsync(Guid classId, AssignTeacherRequest request, CancellationToken ct)
    {
        await EnsureOpenClassAsync(classId, ct);
        await roles.EnsureActiveTeachersAsync(ct, request.TeacherId);
        var from = ValidateFrom(request.FromDate);
        if (request.ToDate is { } to && to < from)
            throw AppException.Invalid("INVALID_RANGE", "Khoảng ngày không hợp lệ");

        var query = db.Sessions.Unlocked().Where(s => s.ClassId == classId && s.SessionDate >= from);
        if (request.ToDate is { } toDate) query = query.Where(s => s.SessionDate <= toDate);
        if (!request.ReplaceExisting) query = query.Where(s => s.MainTeacherId == null);

        var sessions = await query.ToListAsync(ct);
        foreach (var session in sessions)
        {
            session.MainTeacherId = request.TeacherId;
            if (session.CoverTeacherId == request.TeacherId) session.CoverTeacherId = null;
        }
        return await CompleteAsync(classId, sessions.Count, AuditActions.AssignTeacher, new
        {
            request.TeacherId,
            From = from,
            request.ToDate,
            request.ReplaceExisting,
            Sessions = sessions.Count
        }, ct);
    }

    public async Task<StaffChangeResultDto> UnassignAsync(Guid classId, Guid teacherId, DateOnly? fromDate, CancellationToken ct)
    {
        await EnsureOpenClassAsync(classId, ct);
        var from = ValidateFrom(fromDate);
        var sessions = await db.Sessions.Unlocked()
            .Where(s => s.ClassId == classId && s.SessionDate >= from && (s.MainTeacherId == teacherId || s.CoverTeacherId == teacherId))
            .ToListAsync(ct);
        foreach (var session in sessions)
        {
            if (session.MainTeacherId == teacherId) session.MainTeacherId = null;
            if (session.CoverTeacherId == teacherId) session.CoverTeacherId = null;
        }
        return await CompleteAsync(classId, sessions.Count, AuditActions.UnassignTeacher, new { TeacherId = teacherId, From = from, Sessions = sessions.Count }, ct);
    }

    public async Task<StaffChangeResultDto> ChangeMainAsync(Guid classId, ChangeMainTeacherRequest request, CancellationToken ct)
    {
        await EnsureOpenClassAsync(classId, ct);
        await roles.EnsureActiveTeachersAsync(ct, request.TeacherId);
        var from = ValidateFrom(request.EffectiveDate);
        var sessions = await db.Sessions.Unlocked().Where(s => s.ClassId == classId && s.SessionDate >= from).ToListAsync(ct);
        var previous = sessions.Select(s => s.MainTeacherId).OfType<Guid>().Distinct().ToList();
        foreach (var session in sessions)
        {
            session.MainTeacherId = request.TeacherId;
            if (session.CoverTeacherId == request.TeacherId) session.CoverTeacherId = null;
        }
        return await CompleteAsync(classId, sessions.Count, AuditActions.ChangeMainTeacher, new
        {
            PreviousTeacherIds = previous,
            request.TeacherId,
            EffectiveDate = from,
            Sessions = sessions.Count
        }, ct);
    }

    private async Task<StaffChangeResultDto> CompleteAsync(Guid classId, int affected, string action, object meta, CancellationToken ct)
    {
        audit.Record(action, AuditEntities.Class, classId, meta);
        await db.SaveChangesAsync(ct);
        return new StaffChangeResultDto(classId, affected, await ListAsync(classId, ct));
    }

    private DateOnly ValidateFrom(DateOnly? date)
    {
        var from = date ?? clock.Today;
        if (from < clock.Today)
            throw AppException.Invalid("PAST_DATE", "Không thể điều chỉnh phân công cho các buổi đã qua");
        return from;
    }

    private async Task EnsureOpenClassAsync(Guid classId, CancellationToken ct)
    {
        var status = await db.Classes.Where(c => c.Id == classId).Select(c => c.Status).FirstOrDefaultAsync(ct)
                     ?? throw AppException.NotFound("lớp học");
        if (status == ClassStatuses.Closed)
            throw AppException.Conflict("CLASS_CLOSED", "Lớp đã đóng");
    }
}
