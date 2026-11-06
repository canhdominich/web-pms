using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Trials;

internal sealed record TrialScheduleMeta(Guid SessionId, Guid ClassId, DateOnly SessionDate, string? Note);

internal sealed record TrialOutcomeMeta(Guid ScheduleId, string Status, string? Note);

internal sealed record TrialConversionMeta(Guid StudentId, Guid EnrollmentId, Guid ClassId, bool NewStudent, string From);

internal sealed record TrialScheduleRecord(Guid Id, Guid LeadId, Guid SessionId, string Status, string? Note, DateTime CreatedAt);

internal sealed class TrialScheduleBook(IAppDbContext db, IAuditTrail audit)
{
    private static readonly string[] Actions = [AuditActions.TrialSchedule, AuditActions.TrialResult, AuditActions.TrialCancel];

    public async Task<IReadOnlyList<TrialScheduleRecord>> HistoryAsync(IReadOnlyCollection<Guid> leadIds, CancellationToken ct)
    {
        var events = await db.AuditLogs.AsNoTracking()
            .Events(AuditEntities.TrialLead, Actions)
            .Where(a => leadIds.Contains(a.EntityId!.Value))
            .OrderBy(a => a.CreatedAt)
            .Select(a => new { a.Id, LeadId = a.EntityId!.Value, a.Action, a.Meta, a.CreatedAt })
            .ToListAsync(ct);

        var records = new List<TrialScheduleRecord>();
        foreach (var item in events)
        {
            if (item.Action == AuditActions.TrialSchedule)
            {
                var meta = AuditReader.Read<TrialScheduleMeta>(item.Meta);
                records.Add(new TrialScheduleRecord(item.Id, item.LeadId, meta.SessionId, TrialScheduleStatuses.Scheduled, meta.Note, item.CreatedAt));
                continue;
            }
            var outcome = AuditReader.Read<TrialOutcomeMeta>(item.Meta);
            var index = records.FindIndex(r => r.Id == outcome.ScheduleId);
            if (index >= 0)
                records[index] = records[index] with { Status = outcome.Status, Note = Join(records[index].Note, outcome.Note) };
        }
        return records;
    }

    public async Task<TrialScheduleRecord?> ActiveAsync(Guid leadId, CancellationToken ct) =>
        (await HistoryAsync([leadId], ct)).LastOrDefault(r => r.Status == TrialScheduleStatuses.Scheduled);

    public async Task<int> CountScheduledAsync(Guid sessionId, CancellationToken ct)
    {
        var leadIds = await db.TrialLeads.Where(l => l.Status == TrialLeadStatuses.Scheduled).Select(l => l.Id).ToListAsync(ct);
        if (leadIds.Count == 0) return 0;
        return (await HistoryAsync(leadIds, ct)).Count(r => r.Status == TrialScheduleStatuses.Scheduled && r.SessionId == sessionId);
    }

    public void Schedule(Guid leadId, TrialScheduleMeta meta) =>
        audit.Record(AuditActions.TrialSchedule, AuditEntities.TrialLead, leadId, meta);

    public void Close(Guid leadId, TrialScheduleRecord schedule, string status, string? note) =>
        audit.Record(status == TrialScheduleStatuses.Canceled ? AuditActions.TrialCancel : AuditActions.TrialResult,
            AuditEntities.TrialLead, leadId, new TrialOutcomeMeta(schedule.Id, status, note));

    private static string? Join(string? current, string? addition) =>
        addition is null ? current : current is null ? addition : $"{current}{Environment.NewLine}{addition}";
}
