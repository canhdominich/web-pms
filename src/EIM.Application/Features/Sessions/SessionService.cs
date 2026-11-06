using System.Linq.Expressions;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Application.Features.Trials;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Sessions;

public interface ISessionService
{
    Task<IReadOnlyList<SessionDto>> ListByClassAsync(Guid classId, SessionQuery query, CancellationToken ct);
    Task<IReadOnlyList<SessionDto>> ListMineAsync(SessionQuery query, CancellationToken ct);
    Task<IReadOnlyList<SessionDto>> ListByTeacherAsync(Guid teacherId, SessionQuery query, CancellationToken ct);
    Task<SessionDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<SessionDto> UpdateAsync(Guid id, UpdateSessionRequest request, CancellationToken ct);
}

internal static class SessionProjections
{
    public static readonly Expression<Func<Session, SessionDto>> ToDto = s => new SessionDto(
        s.Id, s.ClassId, s.Classroom.Code, s.Classroom.Name, s.Classroom.Room, s.SessionDate, s.UnitNo, s.LessonNo, s.SessionType,
        s.MainTeacherId, s.MainTeacher == null ? null : s.MainTeacher.FullName,
        s.CoverTeacherId, s.CoverTeacher == null ? null : s.CoverTeacher.FullName,
        s.Feedbacks.Count, s.Scores.Count);

    public static async Task<SessionDto> GetDtoAsync(this IAppDbContext db, Guid id, CancellationToken ct) =>
        await db.Sessions.AsNoTracking().Where(s => s.Id == id).Select(ToDto).FirstOrDefaultAsync(ct)
        ?? throw AppException.NotFound("buổi học");

    public static void EnsureCanView(this ICurrentUser user, Session session)
    {
        if (user.IsInAnyRole(RoleSets.Staff)) return;
        var id = user.RequiredId;
        if (session.MainTeacherId != id && session.CoverTeacherId != id)
            throw AppException.Forbidden("Bạn chỉ được xem buổi học mình phụ trách");
    }
}

internal sealed class SessionService(IAppDbContext db, ICurrentUser currentUser, RoleDirectory roles, TrialScheduleBook trials, IAuditTrail audit) : ISessionService
{
    public async Task<IReadOnlyList<SessionDto>> ListByClassAsync(Guid classId, SessionQuery query, CancellationToken ct)
    {
        if (!await db.Classes.AnyAsync(c => c.Id == classId, ct)) throw AppException.NotFound("lớp học");
        return await Filter(db.Sessions.Where(s => s.ClassId == classId), query).ToListAsync(ct);
    }

    public Task<IReadOnlyList<SessionDto>> ListMineAsync(SessionQuery query, CancellationToken ct) =>
        ListByTeacherAsync(currentUser.RequiredId, query, ct);

    public async Task<IReadOnlyList<SessionDto>> ListByTeacherAsync(Guid teacherId, SessionQuery query, CancellationToken ct) =>
        await Filter(db.Sessions.TaughtBy(teacherId), query).ToListAsync(ct);

    public async Task<SessionDetailDto> GetAsync(Guid id, CancellationToken ct)
    {
        var session = await db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct) ?? throw AppException.NotFound("buổi học");
        currentUser.EnsureCanView(session);
        var roster = await db.Enrollments.AsNoTracking()
            .AttendingOn(session.ClassId, session.SessionDate)
            .OrderBy(e => e.Student.FullName)
            .Select(e => new SessionStudentDto(e.StudentId, e.Student.FullName, e.Id, e.Status))
            .ToListAsync(ct);
        return new SessionDetailDto(await db.GetDtoAsync(id, ct), roster, await trials.CountScheduledAsync(id, ct));
    }

    public async Task<SessionDto> UpdateAsync(Guid id, UpdateSessionRequest request, CancellationToken ct)
    {
        var session = await db.Sessions.Include(s => s.Classroom).ThenInclude(c => c.Program).FirstOrDefaultAsync(s => s.Id == id, ct)
                      ?? throw AppException.NotFound("buổi học");
        if (session.Classroom.Status == ClassStatuses.Closed)
            throw AppException.Conflict("CLASS_CLOSED", "Lớp đã đóng");

        var type = Guard.OneOf(request.SessionType, SessionTypes.All, "Loại buổi học");
        var program = session.Classroom.Program;
        if (request.UnitNo > program.TotalUnits || request.LessonNo > program.LessonsPerUnit)
            throw AppException.Invalid("INVALID_LESSON", $"Unit tối đa {program.TotalUnits}, lesson tối đa {program.LessonsPerUnit}");
        if (request.CoverTeacherId is not null && request.CoverTeacherId == request.MainTeacherId)
            throw AppException.Invalid("INVALID_COVER", "Giáo viên dạy thay phải khác giáo viên chính");
        await roles.EnsureActiveTeachersAsync(ct, request.MainTeacherId, request.CoverTeacherId);

        var locked = await db.SessionFeedback.AnyAsync(f => f.SessionId == id, ct) || await db.SessionScores.AnyAsync(s => s.SessionId == id, ct);
        if (locked && (request.SessionDate != session.SessionDate || type != session.SessionType))
            throw AppException.Conflict("SESSION_LOCKED", "Buổi học đã có nhận xét/điểm, không thể đổi ngày hoặc loại buổi");
        if (request.SessionDate != session.SessionDate
            && await db.Sessions.AnyAsync(s => s.ClassId == session.ClassId && s.Id != id && s.SessionDate == request.SessionDate, ct))
            throw AppException.Conflict("SESSION_OVERLAP", "Lớp đã có buổi học khác vào ngày này");

        var before = Snapshot(session);
        session.SessionDate = request.SessionDate;
        session.UnitNo = request.UnitNo;
        session.LessonNo = request.LessonNo;
        session.SessionType = type;
        session.MainTeacherId = request.MainTeacherId;
        session.CoverTeacherId = request.CoverTeacherId;
        audit.Record(AuditActions.Update, AuditEntities.Session, id, new { Before = before, After = Snapshot(session) });
        await db.SaveChangesAsync(ct);
        return await db.GetDtoAsync(id, ct);
    }

    private static IQueryable<SessionDto> Filter(IQueryable<Session> sessions, SessionQuery query)
    {
        if (query.From is { } from) sessions = sessions.Where(s => s.SessionDate >= from);
        if (query.To is { } to) sessions = sessions.Where(s => s.SessionDate <= to);
        if (query.SessionType.Clean() is { } type) sessions = sessions.Where(s => s.SessionType == type.ToUpper());
        return sessions.AsNoTracking().OrderBy(s => s.SessionDate).ThenBy(s => s.Classroom.Code).Select(SessionProjections.ToDto);
    }

    private static object Snapshot(Session s) =>
        new { s.SessionDate, s.UnitNo, s.LessonNo, s.SessionType, s.MainTeacherId, s.CoverTeacherId };
}
