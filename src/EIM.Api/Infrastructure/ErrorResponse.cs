namespace EIM.Api.Infrastructure;

public sealed record ErrorResponse(string Code, string Message, object? Details, string TraceId);
