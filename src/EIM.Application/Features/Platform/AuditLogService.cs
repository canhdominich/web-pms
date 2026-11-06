using System.Text.Json;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Platform;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> ListAsync(AuditLogQuery query, CancellationToken ct);
}

internal sealed class AuditLogService(IAppDbContext db) : IAuditLogService
{
    public async Task<PagedResult<AuditLogDto>> ListAsync(AuditLogQuery query, CancellationToken ct)
    {
        var logs = db.AuditLogs.AsNoTracking();
        if (query.ActorUserId is { } actor) logs = logs.Where(a => a.ActorUserId == actor);
        if (query.Entity.Clean() is { } entity) logs = logs.Where(a => a.Entity == entity);
        if (query.Action.Clean() is { } action) logs = logs.Where(a => a.Action == action.ToUpper());
        if (query.EntityId is { } entityId) logs = logs.Where(a => a.EntityId == entityId);
        if (query.From is { } from) logs = logs.Where(a => a.CreatedAt >= from.ToUniversalTime());
        if (query.To is { } to) logs = logs.Where(a => a.CreatedAt <= to.ToUniversalTime());

        var page = await logs.OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Id, a.ActorUserId, ActorName = a.Actor == null ? null : a.Actor.FullName, a.Action, a.Entity, a.EntityId, a.Meta, a.CreatedAt })
            .ToPagedAsync(query, ct);

        return new PagedResult<AuditLogDto>(
            page.Items.Select(a => new AuditLogDto(a.Id, a.ActorUserId, a.ActorName, a.Action, a.Entity, a.EntityId, Parse(a.Meta), a.CreatedAt)).ToList(),
            page.Page, page.PageSize, page.Total);
    }

    private static JsonElement Parse(string meta)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(meta) ? "{}" : meta);
        return document.RootElement.Clone();
    }
}
