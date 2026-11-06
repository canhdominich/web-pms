namespace EIM.Application.Abstractions;

public interface IAuditTrail
{
    void Record(string action, string entity, Guid? entityId, object? meta = null);
}
