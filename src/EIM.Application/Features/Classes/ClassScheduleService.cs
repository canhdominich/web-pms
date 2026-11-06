using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using EIM.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Classes;

public interface IClassScheduleService
{
    Task<ScheduleDto> GetAsync(Guid classId, CancellationToken ct);
    Task<ScheduleDto> GenerateAsync(Guid classId, GenerateSessionsRequest request, CancellationToken ct);
    Task<ScheduleDto> RescheduleAsync(Guid classId, RescheduleRequest request, CancellationToken ct);
}

internal sealed class ClassScheduleService(IAppDbContext db, RoleDirectory roles, IAuditTrail audit, IClock clock) : IClassScheduleService
{
    public async Task<ScheduleDto> GetAsync(Guid classId, CancellationToken ct)
    {
        if (!await db.Classes.AnyAsync(c => c.Id == classId, ct)) throw AppException.NotFound("lớp học");
        var sessions = await db.Sessions.AsNoTracking()
            .Where(s => s.ClassId == classId)
            .OrderBy(s => s.SessionDate)
            .Select(s => new { s.SessionDate, Locked = s.Feedbacks.Any() || s.Scores.Any() })
            .ToListAsync(ct);

        var today = clock.Today;
        var upcoming = sessions.Where(s => s.SessionDate >= today).Select(s => s.SessionDate).ToList();
        var basis = upcoming.Count > 0 ? upcoming : sessions.Select(s => s.SessionDate).ToList();
        var weekdays = basis.Select(d => d.DayOfWeek).Distinct().OrderBy(d => ((int)d + 6) % 7).ToList();

        return new ScheduleDto(
            classId,
            weekdays,
            sessions.Count,
            sessions.Count(s => s.SessionDate < today),
            sessions.Count(s => s.Locked),
            sessions.FirstOrDefault()?.SessionDate,
            sessions.LastOrDefault()?.SessionDate,
            upcoming.Count > 0 ? upcoming[0] : null);
    }

    public async Task<ScheduleDto> GenerateAsync(Guid classId, GenerateSessionsRequest request, CancellationToken ct)
    {
        var classroom = await db.Classes.Include(c => c.Program).FirstOrDefaultAsync(c => c.Id == classId, ct)
                        ?? throw AppException.NotFound("lớp học");
        EnsureOpen(classroom);
        if (await db.Sessions.AnyAsync(s => s.ClassId == classId, ct))
            throw AppException.Conflict("SESSIONS_EXIST", "Lớp đã có buổi học, hãy dùng chức năng cập nhật lịch học");

        var weekdays = ValidateWeekdays(request.Weekdays, classroom.Program.SessionsPerWeek);
        await roles.EnsureActiveTeachersAsync(ct, request.MainTeacherId);

        var plan = CurriculumPlanner.Sessions(classroom.Program.TotalUnits, classroom.Program.LessonsPerUnit);
        var from = request.FromDate ?? classroom.StartDate;
        if (from < classroom.StartDate)
            throw AppException.Invalid("INVALID_DATE", "Ngày bắt đầu sinh buổi học không được trước ngày khai giảng");

        var dates = WeeklyCalendar.Dates(from, weekdays, plan.Count);
        db.Sessions.AddRange(plan.Select((p, i) => new Session
        {
            ClassId = classId,
            SessionDate = dates[i],
            UnitNo = p.UnitNo,
            LessonNo = p.LessonNo,
            SessionType = p.SessionType,
            MainTeacherId = request.MainTeacherId,
            CreatedAt = clock.UtcNow
        }));
        audit.Record(AuditActions.Generate, AuditEntities.Class, classId, new
        {
            Weekdays = weekdays.Select(w => w.ToString()),
            Sessions = plan.Count,
            From = dates[0],
            To = dates[^1],
            request.MainTeacherId
        });
        await db.SaveChangesAsync(ct);
        return await GetAsync(classId, ct);
    }

    public async Task<ScheduleDto> RescheduleAsync(Guid classId, RescheduleRequest request, CancellationToken ct)
    {
        var classroom = await db.Classes.Include(c => c.Program).FirstOrDefaultAsync(c => c.Id == classId, ct)
                        ?? throw AppException.NotFound("lớp học");
        EnsureOpen(classroom);
        if (request.EffectiveDate < clock.Today)
            throw AppException.Invalid("PAST_DATE", "Ngày hiệu lực không được ở quá khứ");

        var weekdays = ValidateWeekdays(request.Weekdays, classroom.Program.SessionsPerWeek);
        var sessions = await db.Sessions
            .Where(s => s.ClassId == classId && s.SessionDate >= request.EffectiveDate)
            .OrderBy(s => s.SessionDate)
            .ToListAsync(ct);
        if (sessions.Count == 0)
            throw AppException.Conflict("NO_SESSIONS", "Không có buổi học nào từ ngày hiệu lực");
        if (await db.Sessions.AnyAsync(s => s.ClassId == classId && s.SessionDate >= request.EffectiveDate && (s.Feedbacks.Any() || s.Scores.Any()), ct))
            throw AppException.Conflict("SESSION_LOCKED", "Có buổi học sau ngày hiệu lực đã được nhập nhận xét/điểm");

        var dates = WeeklyCalendar.Dates(request.EffectiveDate, weekdays, sessions.Count);
        var before = new { From = sessions[0].SessionDate, To = sessions[^1].SessionDate };
        for (var i = 0; i < sessions.Count; i++) sessions[i].SessionDate = dates[i];

        audit.Record(AuditActions.Reschedule, AuditEntities.Class, classId, new
        {
            Weekdays = weekdays.Select(w => w.ToString()),
            request.EffectiveDate,
            Sessions = sessions.Count,
            Before = before,
            After = new { From = dates[0], To = dates[^1] }
        });
        await db.SaveChangesAsync(ct);
        return await GetAsync(classId, ct);
    }

    private static void EnsureOpen(Classroom classroom)
    {
        if (classroom.Status == ClassStatuses.Closed)
            throw AppException.Conflict("CLASS_CLOSED", "Lớp đã đóng");
    }

    private static IReadOnlyList<DayOfWeek> ValidateWeekdays(IReadOnlyList<DayOfWeek> weekdays, int sessionsPerWeek)
    {
        var distinct = weekdays.Distinct().ToList();
        if (distinct.Any(d => !Enum.IsDefined(d)))
            throw AppException.Invalid("INVALID_WEEKDAY", "Ngày trong tuần không hợp lệ");
        if (distinct.Count != sessionsPerWeek)
            throw AppException.Invalid("WEEKDAYS_MISMATCH", $"Chương trình yêu cầu {sessionsPerWeek} buổi/tuần");
        return distinct;
    }
}
