using System.Text.Json;
using TLighthouse.Domain.Entities;

namespace TLighthouse.Application.Common;

internal static class AuditReader
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static T Read<T>(string meta) => JsonSerializer.Deserialize<T>(meta, Options)
                                           ?? throw new InvalidOperationException("Audit meta is empty");

    public static IQueryable<AuditLog> Events(this IQueryable<AuditLog> logs, string entity, IReadOnlyCollection<string> actions) =>
        logs.Where(a => a.Entity == entity && a.EntityId != null && actions.Contains(a.Action));
}
