using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Sessions;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Assessments;

public interface IAssessmentService
{
    Task<FeedbackFormDto> GetFormAsync(Guid sessionId, CancellationToken ct);
    Task<FeedbackFormDto> SaveFeedbackAsync(Guid sessionId, SaveFeedbackRequest request, CancellationToken ct);
    Task<FeedbackFormDto> SaveScoresAsync(Guid sessionId, SaveScoresRequest request, CancellationToken ct);
    Task<ImportResult> ImportFeedbackAsync(Stream file, CancellationToken ct);
    Task<FileContent> GetTemplateAsync(Guid? sessionId, CancellationToken ct);
    Task<FileContent> ExportAsync(FeedbackExportQuery query, CancellationToken ct);
    Task<IReadOnlyList<StudentAssessmentDto>> GetStudentHistoryAsync(Guid studentId, DateOnly? from, DateOnly? to, CancellationToken ct);
    Task<AttendanceSummaryDto> GetAttendanceAsync(Guid enrollmentId, CancellationToken ct);
}

internal sealed class AssessmentService(
    IAppDbContext db,
    ICurrentUser currentUser,
    IExcelService excel,
    IAuditTrail audit,
    IClock clock) : IAssessmentService
{
    private static readonly string[] TemplateColumns =
        ["session_id", "student_id", "student_name", "attendance", "homework", "participation", "behavior", "comment_text"];

    private sealed record NormalizedFeedback(Guid StudentId, string Attendance, string? Homework, string? Participation, string? Behavior, string? CommentText);

    public async Task<FeedbackFormDto> GetFormAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await LoadSessionAsync(sessionId, ct);
        currentUser.EnsureCanView(session);
        return await BuildFormAsync(session, ct);
    }

    public async Task<FeedbackFormDto> SaveFeedbackAsync(Guid sessionId, SaveFeedbackRequest request, CancellationToken ct)
    {
        var session = await LoadSessionAsync(sessionId, ct);
        ThrowIfBlocked(WriteBlockReason(session));
        EnsureDistinct(request.Items.Select(i => i.StudentId));

        var items = request.Items.Select(i =>
        {
            var errors = new List<string>();
            var normalized = Normalize(i, errors);
            if (errors.Count > 0) throw AppException.Invalid("INVALID_FEEDBACK", string.Join("; ", errors));
            return normalized;
        }).ToList();
        await EnsureStudentsAllowedAsync(session, items.Select(i => i.StudentId), ct);

        var changes = await UpsertFeedbackAsync(session, items, ct);
        audit.Record(AuditActions.Update, AuditEntities.Feedback, session.Id, new { SessionId = session.Id, Changes = changes });
        await db.SaveChangesAsync(ct);
        return await BuildFormAsync(session, ct);
    }

    public async Task<FeedbackFormDto> SaveScoresAsync(Guid sessionId, SaveScoresRequest request, CancellationToken ct)
    {
        var session = await LoadSessionAsync(sessionId, ct);
        ThrowIfBlocked(WriteBlockReason(session));
        if (!SessionTypes.Assessed.Contains(session.SessionType))
            throw AppException.Conflict("NOT_ASSESSED_SESSION", "Chỉ nhập đánh giá cho buổi đánh giá (TEST/MIDTERM/FINAL)");
        EnsureDistinct(request.Items.Select(i => i.StudentId));
        await EnsureStudentsAllowedAsync(session, request.Items.Select(i => i.StudentId), ct);

        var ids = request.Items.Select(i => i.StudentId).ToList();
        var existing = await db.SessionScores.Where(s => s.SessionId == sessionId && ids.Contains(s.StudentId)).ToDictionaryAsync(s => s.StudentId, ct);
        var changes = new List<object>();
        var now = clock.UtcNow;

        foreach (var item in request.Items)
        {
            var skills = new[] { item.Language, item.Cognitive, item.Physical, item.Social }.OfType<int>().ToList();
            var total = item.Total ?? (skills.Count > 0 ? (int)Math.Round(skills.Average(), MidpointRounding.AwayFromZero) : null as int?)
                        ?? throw AppException.Invalid("SCORE_REQUIRED", "Cần nhập ít nhất một đánh giá thành phần hoặc tổng");

            if (!existing.TryGetValue(item.StudentId, out var score))
            {
                score = new SessionScore { SessionId = sessionId, StudentId = item.StudentId, CreatedAt = now };
                db.SessionScores.Add(score);
            }
            var before = existing.ContainsKey(item.StudentId) ? ScoreSnapshot(score) : null;
            score.ScoreType = session.SessionType;
            score.Language = item.Language;
            score.Cognitive = item.Cognitive;
            score.Physical = item.Physical;
            score.Social = item.Social;
            score.Total = total;
            score.UpdatedAt = now;
            changes.Add(new { item.StudentId, Before = before, After = ScoreSnapshot(score) });
        }

        audit.Record(AuditActions.Update, AuditEntities.Score, sessionId, new { SessionId = sessionId, Changes = changes });
        await db.SaveChangesAsync(ct);
        return await BuildFormAsync(session, ct);
    }

    public async Task<ImportResult> ImportFeedbackAsync(Stream file, CancellationToken ct)
    {
        var rows = excel.Read(file);
        if (rows.Count == 0) return new ImportResult(0, [new ImportRowError(0, "Tệp không có dữ liệu")]);

        var errors = new List<ImportRowError>();
        var parsed = new List<(int Row, Guid SessionId, NormalizedFeedback Item)>();
        foreach (var row in rows)
        {
            var rowErrors = new List<string>();
            var sessionOk = Guid.TryParse(row.Get("session_id"), out var sid);
            var studentOk = Guid.TryParse(row.Get("student_id"), out var stid);
            if (!sessionOk) rowErrors.Add("session_id không hợp lệ");
            if (!studentOk) rowErrors.Add("student_id không hợp lệ");
            var item = Normalize(new FeedbackItem
            {
                StudentId = stid,
                Attendance = row.Get("attendance") ?? "",
                Homework = row.Get("homework"),
                Participation = row.Get("participation"),
                Behavior = row.Get("behavior"),
                CommentText = row.Get("comment_text")
            }, rowErrors);
            if (sessionOk && studentOk && parsed.Any(p => p.SessionId == sid && p.Item.StudentId == stid))
                rowErrors.Add("Trùng cặp session_id/student_id trong tệp");

            if (rowErrors.Count > 0) errors.Add(new ImportRowError(row.Number, string.Join("; ", rowErrors)));
            else parsed.Add((row.Number, sid, item));
        }

        var sessionIds = parsed.Select(p => p.SessionId).Distinct().ToList();
        var sessions = await db.Sessions.AsNoTracking().Include(s => s.Classroom)
            .Where(s => sessionIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, ct);
        var allowed = new Dictionary<Guid, HashSet<Guid>>();
        foreach (var session in sessions.Values) allowed[session.Id] = await AllowedStudentsAsync(session, ct);

        foreach (var (row, sessionId, item) in parsed)
        {
            if (!sessions.TryGetValue(sessionId, out var session))
                errors.Add(new ImportRowError(row, "Không tìm thấy buổi học"));
            else if (WriteBlockReason(session) is { } reason)
                errors.Add(new ImportRowError(row, reason));
            else if (!allowed[sessionId].Contains(item.StudentId))
                errors.Add(new ImportRowError(row, "Học sinh không thuộc danh sách lớp của buổi học"));
        }

        if (errors.Count > 0) return new ImportResult(0, errors.OrderBy(e => e.Row).ToList());

        foreach (var group in parsed.GroupBy(p => p.SessionId))
            await UpsertFeedbackAsync(sessions[group.Key], group.Select(g => g.Item).ToList(), ct);
        audit.Record(AuditActions.Import, AuditEntities.Feedback, null, new { Rows = parsed.Count, SessionIds = sessionIds });
        await db.SaveChangesAsync(ct);
        return new ImportResult(parsed.Count, []);
    }

    public async Task<FileContent> GetTemplateAsync(Guid? sessionId, CancellationToken ct)
    {
        var rows = new List<IReadOnlyList<object?>>();
        var name = "feedback-template.xlsx";
        if (sessionId is { } id)
        {
            var session = await LoadSessionAsync(id, ct);
            currentUser.EnsureCanView(session);
            var form = await BuildFormAsync(session, ct);
            rows.AddRange(form.Rows.Select(r => (IReadOnlyList<object?>)
            [
                id, r.StudentId, r.StudentName, r.Feedback?.Attendance, r.Feedback?.Homework,
                r.Feedback?.Participation, r.Feedback?.Behavior, r.Feedback?.CommentText
            ]));
            name = $"feedback-template-{session.Classroom.Code}-{session.SessionDate:yyyyMMdd}.xlsx";
        }

        var guide = new ExcelSheet("Guide", ["column", "required", "allowed_values"],
        [
            ["session_id", "yes", "UUID buổi học"],
            ["student_id", "yes", "UUID học sinh"],
            ["attendance", "yes", string.Join(", ", AttendanceValues.All)],
            ["homework", "no", string.Join(", ", HomeworkValues.All)],
            ["participation", "no", "Văn bản tối đa 1000 ký tự"],
            ["behavior", "no", "Văn bản tối đa 1000 ký tự"],
            ["comment_text", "no", "Văn bản tối đa 4000 ký tự"]
        ]);
        return new FileContent(excel.Write(new ExcelSheet("Feedback", TemplateColumns, rows), guide), name);
    }

    public async Task<FileContent> ExportAsync(FeedbackExportQuery query, CancellationToken ct)
    {
        if (query.ClassId is null && query.SessionId is null)
            throw AppException.Invalid("FILTER_REQUIRED", "Cần chọn lớp hoặc buổi học để xuất dữ liệu");

        var sessionsQuery = db.Sessions.AsNoTracking();
        if (!currentUser.IsInAnyRole(RoleSets.AssessmentViewers)) sessionsQuery = sessionsQuery.TaughtBy(currentUser.RequiredId);
        if (query.SessionId is { } sessionId) sessionsQuery = sessionsQuery.Where(s => s.Id == sessionId);
        if (query.ClassId is { } classId) sessionsQuery = sessionsQuery.Where(s => s.ClassId == classId);
        if (query.From is { } from) sessionsQuery = sessionsQuery.Where(s => s.SessionDate >= from);
        if (query.To is { } to) sessionsQuery = sessionsQuery.Where(s => s.SessionDate <= to);

        var sessions = await sessionsQuery
            .Select(s => new { s.Id, ClassCode = s.Classroom.Code, s.SessionDate, s.UnitNo, s.LessonNo, s.SessionType })
            .ToDictionaryAsync(s => s.Id, ct);
        var ids = sessions.Keys.ToList();
        var feedback = await db.SessionFeedback.AsNoTracking().Where(f => ids.Contains(f.SessionId))
            .Select(f => new { Item = f, StudentName = f.Student.FullName, TeacherName = f.Teacher.FullName })
            .ToDictionaryAsync(f => (f.Item.SessionId, f.Item.StudentId), ct);
        var scores = await db.SessionScores.AsNoTracking().Where(s => ids.Contains(s.SessionId))
            .Select(s => new { Item = s, StudentName = s.Student.FullName })
            .ToDictionaryAsync(s => (s.Item.SessionId, s.Item.StudentId), ct);

        var rows = feedback.Keys.Union(scores.Keys)
            .Select(key =>
            {
                var session = sessions[key.SessionId];
                var f = feedback.GetValueOrDefault(key);
                var s = scores.GetValueOrDefault(key);
                return new
                {
                    session.SessionDate,
                    StudentName = f?.StudentName ?? s!.StudentName,
                    Values = (IReadOnlyList<object?>)
                    [
                        session.ClassCode, session.SessionDate, session.UnitNo, session.LessonNo, session.SessionType,
                        key.SessionId, key.StudentId, f?.StudentName ?? s!.StudentName,
                        f?.Item.Attendance, f?.Item.Homework, f?.Item.Participation, f?.Item.Behavior, f?.Item.CommentText, f?.TeacherName,
                        s?.Item.ScoreType, s?.Item.Language, s?.Item.Cognitive, s?.Item.Physical, s?.Item.Social, s?.Item.Total
                    ]
                };
            })
            .OrderBy(r => r.SessionDate).ThenBy(r => r.StudentName)
            .Select(r => r.Values)
            .ToList();

        audit.Record(AuditActions.Export, AuditEntities.Feedback, null, new { query.ClassId, query.SessionId, query.From, query.To, Rows = rows.Count });
        await db.SaveChangesAsync(ct);

        var sheet = new ExcelSheet("Feedback",
        [
            "class_code", "session_date", "unit_no", "lesson_no", "session_type", "session_id", "student_id", "student_name",
            "attendance", "homework", "participation", "behavior", "comment_text", "teacher",
            "score_type", "language", "cognitive", "physical", "social", "total"
        ], rows);
        return new FileContent(excel.Write(sheet), $"feedback-export-{clock.Today:yyyyMMdd}.xlsx");
    }

    public async Task<IReadOnlyList<StudentAssessmentDto>> GetStudentHistoryAsync(Guid studentId, DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        if (!await db.Students.AnyAsync(s => s.Id == studentId, ct)) throw AppException.NotFound("học sinh");

        var feedbackQuery = db.SessionFeedback.AsNoTracking().Where(f => f.StudentId == studentId);
        var scoreQuery = db.SessionScores.AsNoTracking().Where(s => s.StudentId == studentId);
        if (from is { } f0)
        {
            feedbackQuery = feedbackQuery.Where(f => f.Session.SessionDate >= f0);
            scoreQuery = scoreQuery.Where(s => s.Session.SessionDate >= f0);
        }
        if (to is { } t0)
        {
            feedbackQuery = feedbackQuery.Where(f => f.Session.SessionDate <= t0);
            scoreQuery = scoreQuery.Where(s => s.Session.SessionDate <= t0);
        }

        var feedback = await feedbackQuery.ToDictionaryAsync(f => f.SessionId, ct);
        var scores = await scoreQuery.ToDictionaryAsync(s => s.SessionId, ct);
        var sessionIds = feedback.Keys.Union(scores.Keys).ToList();
        var sessions = await db.Sessions.AsNoTracking().Where(s => sessionIds.Contains(s.Id))
            .Select(s => new { s.Id, s.ClassId, s.Classroom.Code, s.SessionDate, s.UnitNo, s.LessonNo, s.SessionType })
            .ToListAsync(ct);

        return sessions
            .OrderByDescending(s => s.SessionDate)
            .Select(s => new StudentAssessmentDto(s.Id, s.ClassId, s.Code, s.SessionDate, s.UnitNo, s.LessonNo, s.SessionType,
                feedback.TryGetValue(s.Id, out var fb) ? ToDto(fb) : null,
                scores.TryGetValue(s.Id, out var sc) ? ToDto(sc) : null))
            .ToList();
    }

    public async Task<AttendanceSummaryDto> GetAttendanceAsync(Guid enrollmentId, CancellationToken ct)
    {
        var enrollment = await db.Enrollments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == enrollmentId, ct)
                         ?? throw AppException.NotFound("ghi danh");
        var to = enrollment.EndDate is { } end && end < clock.Today ? end : clock.Today;
        var sessions = db.Sessions.Where(s => s.ClassId == enrollment.ClassId && s.SessionDate >= enrollment.StartDate && s.SessionDate <= to);

        var total = await sessions.CountAsync(ct);
        var counts = await db.SessionFeedback.AsNoTracking()
            .Where(f => f.StudentId == enrollment.StudentId && sessions.Select(s => s.Id).Contains(f.SessionId) && f.Attendance != null)
            .GroupBy(f => f.Attendance!)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, ct);

        var present = counts.GetValueOrDefault(AttendanceValues.Present);
        var late = counts.GetValueOrDefault(AttendanceValues.Late);
        var absent = counts.GetValueOrDefault(AttendanceValues.Absent);
        var recorded = present + late + absent;
        var rate = recorded == 0 ? 0 : Math.Round((present + late) * 100.0 / recorded, 1);
        return new AttendanceSummaryDto(enrollment.Id, enrollment.StudentId, enrollment.ClassId, enrollment.StartDate, to,
            total, recorded, present, late, absent, Math.Max(0, total - recorded), rate);
    }

    private async Task<Session> LoadSessionAsync(Guid id, CancellationToken ct) =>
        await db.Sessions.AsNoTracking().Include(s => s.Classroom).FirstOrDefaultAsync(s => s.Id == id, ct)
        ?? throw AppException.NotFound("buổi học");

    private string? WriteBlockReason(Session session)
    {
        if (!currentUser.IsInRole(RoleCodes.Teacher) || session.EffectiveTeacherId != currentUser.RequiredId)
            return "Chỉ giáo viên dạy hoặc dạy thay buổi học mới được nhập nhận xét/đánh giá";
        if (session.Classroom.Status == ClassStatuses.Closed)
            return "Lớp đã đóng, dữ liệu đánh giá đã được chốt";
        if (session.SessionDate > clock.Today)
            return "Chưa đến ngày học, không thể nhập nhận xét/đánh giá";
        return null;
    }

    private static void ThrowIfBlocked(string? reason)
    {
        if (reason is not null) throw AppException.Forbidden(reason);
    }

    private static void EnsureDistinct(IEnumerable<Guid> studentIds)
    {
        var ids = studentIds.ToList();
        if (ids.Distinct().Count() != ids.Count)
            throw AppException.Invalid("DUPLICATE_STUDENT", "Danh sách có học sinh bị trùng");
    }

    private async Task<HashSet<Guid>> AllowedStudentsAsync(Session session, CancellationToken ct)
    {
        var roster = await db.Enrollments.AttendingOn(session.ClassId, session.SessionDate).Select(e => e.StudentId).ToListAsync(ct);
        var recorded = await db.SessionFeedback.Where(f => f.SessionId == session.Id).Select(f => f.StudentId)
            .Union(db.SessionScores.Where(s => s.SessionId == session.Id).Select(s => s.StudentId))
            .ToListAsync(ct);
        return roster.Concat(recorded).ToHashSet();
    }

    private async Task EnsureStudentsAllowedAsync(Session session, IEnumerable<Guid> studentIds, CancellationToken ct)
    {
        var allowed = await AllowedStudentsAsync(session, ct);
        if (studentIds.Any(id => !allowed.Contains(id)))
            throw AppException.Invalid("STUDENT_NOT_IN_SESSION", "Có học sinh không thuộc danh sách lớp của buổi học");
    }

    private async Task<List<object>> UpsertFeedbackAsync(Session session, IReadOnlyCollection<NormalizedFeedback> items, CancellationToken ct)
    {
        var ids = items.Select(i => i.StudentId).ToList();
        var existing = await db.SessionFeedback.Where(f => f.SessionId == session.Id && ids.Contains(f.StudentId))
            .ToDictionaryAsync(f => f.StudentId, ct);
        var changes = new List<object>();
        var now = clock.UtcNow;

        foreach (var item in items)
        {
            var found = existing.TryGetValue(item.StudentId, out var feedback);
            if (!found)
            {
                feedback = new SessionFeedback { SessionId = session.Id, StudentId = item.StudentId, CreatedAt = now };
                db.SessionFeedback.Add(feedback);
            }
            var before = found ? FeedbackSnapshot(feedback!) : null;
            feedback!.Attendance = item.Attendance;
            feedback.Homework = item.Homework;
            feedback.Participation = item.Participation;
            feedback.Behavior = item.Behavior;
            feedback.CommentText = item.CommentText;
            feedback.TeacherId = currentUser.RequiredId;
            feedback.UpdatedAt = now;
            changes.Add(new { item.StudentId, Before = before, After = FeedbackSnapshot(feedback) });
        }
        return changes;
    }

    private async Task<FeedbackFormDto> BuildFormAsync(Session session, CancellationToken ct)
    {
        var roster = await db.Enrollments.AsNoTracking().AttendingOn(session.ClassId, session.SessionDate)
            .Select(e => new { e.StudentId, e.Student.FullName })
            .ToListAsync(ct);
        var feedback = await db.SessionFeedback.AsNoTracking().Where(f => f.SessionId == session.Id)
            .Select(f => new { Item = f, f.Student.FullName }).ToListAsync(ct);
        var scores = await db.SessionScores.AsNoTracking().Where(s => s.SessionId == session.Id)
            .Select(s => new { Item = s, s.Student.FullName }).ToListAsync(ct);

        var students = roster.Select(r => (r.StudentId, r.FullName))
            .Concat(feedback.Select(f => (f.Item.StudentId, f.FullName)))
            .Concat(scores.Select(s => (s.Item.StudentId, s.FullName)))
            .DistinctBy(s => s.StudentId)
            .OrderBy(s => s.FullName);

        var rows = students.Select(s => new FeedbackFormRowDto(
                s.StudentId,
                s.FullName,
                feedback.FirstOrDefault(f => f.Item.StudentId == s.StudentId)?.Item is { } fb ? ToDto(fb) : null,
                scores.FirstOrDefault(x => x.Item.StudentId == s.StudentId)?.Item is { } sc ? ToDto(sc) : null))
            .ToList();

        return new FeedbackFormDto(await db.GetDtoAsync(session.Id, ct), SessionTypes.Assessed.Contains(session.SessionType),
            WriteBlockReason(session) is null, rows);
    }

    private static NormalizedFeedback Normalize(FeedbackItem item, List<string> errors)
    {
        var attendance = item.Attendance.Trim().ToUpperInvariant();
        if (!AttendanceValues.All.Contains(attendance))
            errors.Add($"attendance phải là một trong: {string.Join(", ", AttendanceValues.All)}");
        var homework = item.Homework.Clean()?.ToUpperInvariant();
        if (homework is not null && !HomeworkValues.All.Contains(homework))
            errors.Add($"homework phải là một trong: {string.Join(", ", HomeworkValues.All)}");
        var participation = item.Participation.Clean();
        var behavior = item.Behavior.Clean();
        var comment = item.CommentText.Clean();
        if (participation?.Length > 1000 || behavior?.Length > 1000) errors.Add("participation/behavior tối đa 1000 ký tự");
        if (comment?.Length > 4000) errors.Add("comment_text tối đa 4000 ký tự");
        return new NormalizedFeedback(item.StudentId, attendance, homework, participation, behavior, comment);
    }

    private static object FeedbackSnapshot(SessionFeedback f) =>
        new { f.Attendance, f.Homework, f.Participation, f.Behavior, f.CommentText, f.TeacherId };

    private static object ScoreSnapshot(SessionScore s) =>
        new { s.ScoreType, s.Language, s.Cognitive, s.Physical, s.Social, s.Total };

    private static FeedbackDto ToDto(SessionFeedback f) =>
        new(f.Id, f.SessionId, f.StudentId, f.Attendance, f.Homework, f.Participation, f.Behavior, f.CommentText, f.TeacherId, f.CreatedAt, f.UpdatedAt);

    private static ScoreDto ToDto(SessionScore s) =>
        new(s.Id, s.SessionId, s.StudentId, s.ScoreType, s.Language, s.Cognitive, s.Physical, s.Social, s.Total, s.CreatedAt, s.UpdatedAt);
}
