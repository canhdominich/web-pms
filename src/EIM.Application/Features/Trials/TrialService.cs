using System.Linq.Expressions;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Application.Features.Enrollments;
using EIM.Application.Features.Students;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Trials;

public interface ITrialService
{
    Task<PagedResult<TrialLeadDto>> ListAsync(TrialLeadQuery query, CancellationToken ct);
    Task<TrialLeadDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<TrialLeadDto> CreateAsync(SaveTrialLeadRequest request, CancellationToken ct);
    Task<TrialLeadDto> UpdateAsync(Guid id, SaveTrialLeadRequest request, CancellationToken ct);
    Task<TrialLeadDto> ChangeStatusAsync(Guid id, ChangeTrialStatusRequest request, CancellationToken ct);
    Task<FileContent> ExportAsync(TrialLeadQuery query, CancellationToken ct);
    Task<TrialLeadDetailDto> ScheduleAsync(Guid leadId, ScheduleTrialRequest request, CancellationToken ct);
    Task<TrialLeadDetailDto> RecordResultAsync(Guid leadId, TrialResultRequest request, CancellationToken ct);
    Task<TrialLeadDetailDto> CancelScheduleAsync(Guid leadId, CancellationToken ct);
    Task<ConvertTrialResultDto> ConvertAsync(Guid leadId, ConvertTrialRequest request, CancellationToken ct);
}

internal sealed class TrialService(
    IAppDbContext db,
    ICurrentUser currentUser,
    Enroller enroller,
    TrialScheduleBook schedules,
    IExcelService excel,
    IAuditTrail audit,
    IClock clock) : ITrialService
{
    private static readonly Expression<Func<TrialLead, TrialLeadDto>> LeadProjection = l => new TrialLeadDto(
        l.Id, l.FullName, l.Phone, l.Email, l.Source, l.Status, l.Note, l.CreatedBy, l.Creator.FullName, l.CreatedAt);

    public Task<PagedResult<TrialLeadDto>> ListAsync(TrialLeadQuery query, CancellationToken ct) =>
        Filter(query).Select(LeadProjection).ToPagedAsync(query, ct);

    public async Task<TrialLeadDetailDto> GetAsync(Guid id, CancellationToken ct)
    {
        var lead = await GetLeadDtoAsync(id, ct);
        var history = await schedules.HistoryAsync([id], ct);
        var sessionIds = history.Select(h => h.SessionId).Distinct().ToList();
        var sessions = await db.Sessions.AsNoTracking()
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => new { s.Id, s.ClassId, s.Classroom.Code, s.SessionDate, s.UnitNo, s.LessonNo })
            .ToDictionaryAsync(s => s.Id, ct);

        var items = history
            .Where(h => sessions.ContainsKey(h.SessionId))
            .OrderByDescending(h => h.CreatedAt)
            .Select(h =>
            {
                var s = sessions[h.SessionId];
                return new TrialScheduleDto(h.Id, id, h.SessionId, s.ClassId, s.Code, s.SessionDate, s.UnitNo, s.LessonNo, h.Status, h.Note, h.CreatedAt);
            })
            .ToList();

        var conversion = await db.AuditLogs.AsNoTracking()
            .Events(AuditEntities.TrialLead, [AuditActions.Convert])
            .Where(a => a.EntityId == id)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Id, a.ActorUserId, a.Meta, a.CreatedAt })
            .FirstOrDefaultAsync(ct);
        TrialConversionDto? conversionDto = null;
        if (conversion is not null)
        {
            var meta = AuditReader.Read<TrialConversionMeta>(conversion.Meta);
            conversionDto = new TrialConversionDto(conversion.Id, id, meta.StudentId, meta.EnrollmentId, meta.ClassId, conversion.ActorUserId, conversion.CreatedAt);
        }

        return new TrialLeadDetailDto(lead, items, conversionDto);
    }

    public async Task<TrialLeadDto> CreateAsync(SaveTrialLeadRequest request, CancellationToken ct)
    {
        var lead = new TrialLead { CreatedBy = currentUser.RequiredId, Status = TrialLeadStatuses.New, CreatedAt = clock.UtcNow };
        Apply(lead, request);
        db.TrialLeads.Add(lead);
        audit.Record(AuditActions.Create, AuditEntities.TrialLead, lead.Id, Snapshot(lead));
        await db.SaveChangesAsync(ct);
        return await GetLeadDtoAsync(lead.Id, ct);
    }

    public async Task<TrialLeadDto> UpdateAsync(Guid id, SaveTrialLeadRequest request, CancellationToken ct)
    {
        var lead = await FindLeadAsync(id, ct);
        if (lead.Status == TrialLeadStatuses.Converted)
            throw AppException.Conflict("LEAD_CONVERTED", "Lead đã chuyển đổi, không thể chỉnh sửa");
        var before = Snapshot(lead);
        Apply(lead, request);
        audit.Record(AuditActions.Update, AuditEntities.TrialLead, id, new { Before = before, After = Snapshot(lead) });
        await db.SaveChangesAsync(ct);
        return await GetLeadDtoAsync(id, ct);
    }

    public async Task<TrialLeadDto> ChangeStatusAsync(Guid id, ChangeTrialStatusRequest request, CancellationToken ct)
    {
        var lead = await FindLeadAsync(id, ct);
        var status = Guard.OneOf(request.Status, TrialLeadStatuses.Manual, "Trạng thái");
        EnsureOpen(lead);
        if (await schedules.ActiveAsync(id, ct) is { } active)
        {
            if (status != TrialLeadStatuses.Closed)
                throw AppException.Conflict("TRIAL_ALREADY_SCHEDULED", "Lead đang có lịch học thử, hãy cập nhật kết quả hoặc hủy lịch");
            schedules.Close(id, active, TrialScheduleStatuses.Canceled, "Đóng lead");
        }

        var from = lead.Status;
        lead.Status = status;
        lead.Note = AppendNote(lead.Note, request.Note);
        audit.Record(AuditActions.StatusChange, AuditEntities.TrialLead, id, new { From = from, To = status, Note = request.Note.Clean() });
        await db.SaveChangesAsync(ct);
        return await GetLeadDtoAsync(id, ct);
    }

    public async Task<FileContent> ExportAsync(TrialLeadQuery query, CancellationToken ct)
    {
        var leads = await Filter(query).Select(LeadProjection).ToListAsync(ct);
        var sheet = new ExcelSheet("TrialLeads",
            ["id", "full_name", "phone", "email", "source", "status", "note", "created_by", "created_at"],
            leads.Select(l => (IReadOnlyList<object?>)[l.Id, l.FullName, l.Phone, l.Email, l.Source, l.Status, l.Note, l.CreatedByName, l.CreatedAt]));
        audit.Record(AuditActions.Export, AuditEntities.TrialLead, null, new { query.Search, query.Status, query.Source, Rows = leads.Count });
        await db.SaveChangesAsync(ct);
        return new FileContent(excel.Write(sheet), $"trial-leads-{clock.Today:yyyyMMdd}.xlsx");
    }

    public Task<TrialLeadDetailDto> ScheduleAsync(Guid leadId, ScheduleTrialRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var lead = await FindLeadAsync(leadId, ct);
            EnsureOpen(lead);
            if (lead.Status == TrialLeadStatuses.Scheduled)
                throw AppException.Conflict("TRIAL_ALREADY_SCHEDULED", "Lead đang có lịch học thử chưa hoàn tất");

            var session = await db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.SessionId, ct)
                          ?? throw AppException.NotFound("buổi học");
            if (session.SessionDate < clock.Today)
                throw AppException.Invalid("PAST_SESSION", "Không thể xếp lịch học thử vào buổi đã qua");
            if (session.SessionType != SessionTypes.Normal)
                throw AppException.Conflict("ASSESSMENT_SESSION", "Không xếp học thử vào buổi kiểm tra");

            var classroom = await db.LockClassAsync(session.ClassId, ct) ?? throw AppException.NotFound("lớp học");
            if (classroom.Status != ClassStatuses.Active)
                throw AppException.Conflict("CLASS_NOT_ACTIVE", "Lớp không ở trạng thái hoạt động");

            var attending = await db.Enrollments.AttendingOn(session.ClassId, session.SessionDate).CountAsync(ct);
            if (attending + await schedules.CountScheduledAsync(session.Id, ct) >= classroom.Capacity)
                throw AppException.Conflict("SESSION_FULL", "Buổi học đã đủ sĩ số, không thể xếp thêm học thử");

            var from = lead.Status;
            lead.Status = TrialLeadStatuses.Scheduled;
            schedules.Schedule(leadId, new TrialScheduleMeta(session.Id, session.ClassId, session.SessionDate, request.Note.Clean()));
            audit.Record(AuditActions.StatusChange, AuditEntities.TrialLead, leadId, new { From = from, To = lead.Status });
            await db.SaveChangesAsync(ct);
            return await GetAsync(leadId, ct);
        }, ct);

    public async Task<TrialLeadDetailDto> RecordResultAsync(Guid leadId, TrialResultRequest request, CancellationToken ct)
    {
        var lead = await FindLeadAsync(leadId, ct);
        var active = await RequireActiveAsync(leadId, ct);
        var sessionDate = await db.Sessions.Where(s => s.Id == active.SessionId).Select(s => (DateOnly?)s.SessionDate).FirstOrDefaultAsync(ct);
        if (sessionDate > clock.Today)
            throw AppException.Conflict("SESSION_NOT_STARTED", "Chưa đến ngày học thử");

        var status = Guard.OneOf(request.Status, TrialScheduleStatuses.Results, "Kết quả");
        schedules.Close(leadId, active, status, request.Note.Clean());
        lead.Status = status == TrialScheduleStatuses.Attended ? TrialLeadStatuses.Attended : TrialLeadStatuses.NoShow;
        lead.Note = AppendNote(lead.Note, request.Note);
        audit.Record(AuditActions.StatusChange, AuditEntities.TrialLead, leadId, new { From = TrialLeadStatuses.Scheduled, To = lead.Status });
        await db.SaveChangesAsync(ct);
        return await GetAsync(leadId, ct);
    }

    public async Task<TrialLeadDetailDto> CancelScheduleAsync(Guid leadId, CancellationToken ct)
    {
        var lead = await FindLeadAsync(leadId, ct);
        var active = await RequireActiveAsync(leadId, ct);
        schedules.Close(leadId, active, TrialScheduleStatuses.Canceled, null);
        lead.Status = TrialLeadStatuses.Contacted;
        audit.Record(AuditActions.StatusChange, AuditEntities.TrialLead, leadId, new { From = TrialLeadStatuses.Scheduled, To = lead.Status });
        await db.SaveChangesAsync(ct);
        return await GetAsync(leadId, ct);
    }

    public Task<ConvertTrialResultDto> ConvertAsync(Guid leadId, ConvertTrialRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var lead = await FindLeadAsync(leadId, ct);
            EnsureOpen(lead);

            Student student;
            if (request.ExistingStudentId is { } existingId)
            {
                student = await db.Students.FirstOrDefaultAsync(s => s.Id == existingId, ct) ?? throw AppException.NotFound("học viên");
            }
            else
            {
                student = new Student { FullName = lead.FullName, Phone = lead.Phone, Email = lead.Email, CreatedAt = clock.UtcNow };
                StudentService.ApplyProfile(student, request);
                db.Students.Add(student);
                audit.Record(AuditActions.Create, AuditEntities.Student, student.Id, StudentService.Snapshot(student));
                await db.SaveChangesAsync(ct);
            }

            var enrollment = (await enroller.EnrollAsync(request.ClassId, [student.Id], request.StartDate, ct))[0];
            if (await schedules.ActiveAsync(leadId, ct) is { } active)
                schedules.Close(leadId, active, TrialScheduleStatuses.Canceled, "Đã chuyển đổi");

            var from = lead.Status;
            lead.Status = TrialLeadStatuses.Converted;
            audit.Record(AuditActions.Enroll, AuditEntities.Enrollment, enrollment.Id, new { enrollment.StudentId, enrollment.ClassId, LeadId = leadId });
            audit.Record(AuditActions.Convert, AuditEntities.TrialLead, leadId,
                new TrialConversionMeta(student.Id, enrollment.Id, request.ClassId, request.ExistingStudentId is null, from));
            await db.SaveChangesAsync(ct);

            var detail = await GetAsync(leadId, ct);
            var studentDto = await db.Students.AsNoTracking().Where(s => s.Id == student.Id).Select(StudentService.Projection).FirstAsync(ct);
            var enrollmentDto = (await db.ToDtosAsync([enrollment.Id], ct))[0];
            return new ConvertTrialResultDto(detail.Conversion!, studentDto, enrollmentDto);
        }, ct);

    private IQueryable<TrialLead> Filter(TrialLeadQuery query)
    {
        var leads = db.TrialLeads.AsNoTracking();
        if (query.Search.Clean() is { } search)
            leads = leads.Where(l => l.FullName.Contains(search) || l.Phone.StartsWith(search) || (l.Email != null && l.Email.Contains(search)));
        if (query.Status.Clean() is { } status) leads = leads.Where(l => l.Status == status.ToUpper());
        if (query.Source.Clean() is { } source) leads = leads.Where(l => l.Source == source);
        return leads.OrderByDescending(l => l.CreatedAt);
    }

    private async Task<TrialScheduleRecord> RequireActiveAsync(Guid leadId, CancellationToken ct) =>
        await schedules.ActiveAsync(leadId, ct) ?? throw AppException.Conflict("NO_ACTIVE_SCHEDULE", "Lead không có lịch học thử đang chờ");

    private async Task<TrialLead> FindLeadAsync(Guid id, CancellationToken ct) =>
        await db.TrialLeads.FirstOrDefaultAsync(l => l.Id == id, ct) ?? throw AppException.NotFound("lead học thử");

    private async Task<TrialLeadDto> GetLeadDtoAsync(Guid id, CancellationToken ct) =>
        await db.TrialLeads.AsNoTracking().Where(l => l.Id == id).Select(LeadProjection).FirstOrDefaultAsync(ct)
        ?? throw AppException.NotFound("lead học thử");

    private static void EnsureOpen(TrialLead lead)
    {
        if (TrialLeadStatuses.Final.Contains(lead.Status))
            throw AppException.Conflict("LEAD_FINALIZED", "Lead đã chuyển đổi hoặc đã đóng");
    }

    private static void Apply(TrialLead lead, SaveTrialLeadRequest request)
    {
        lead.FullName = request.FullName.Trim();
        lead.Phone = request.Phone.Trim();
        lead.Email = request.Email.Clean()?.ToLowerInvariant();
        lead.Source = request.Source.Clean();
        lead.Note = request.Note.Clean();
    }

    private string? AppendNote(string? current, string? addition) =>
        addition.Clean() is { } note ? string.Join(Environment.NewLine, new[] { current, $"[{clock.UtcNow:yyyy-MM-dd HH:mm}] {note}" }.OfType<string>()) : current;

    private static object Snapshot(TrialLead l) => new { l.FullName, l.Phone, l.Email, l.Source, l.Status, l.Note };
}
