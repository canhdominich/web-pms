namespace TLighthouse.Application.Common;

public enum ErrorKind
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict
}

public sealed class AppException(ErrorKind kind, string code, string message, object? details = null) : Exception(message)
{
    public ErrorKind Kind { get; } = kind;
    public string Code { get; } = code;
    public object? Details { get; } = details;

    public static AppException NotFound(string entity) => new(ErrorKind.NotFound, "NOT_FOUND", $"Không tìm thấy {entity}");
    public static AppException Conflict(string code, string message) => new(ErrorKind.Conflict, code, message);
    public static AppException Invalid(string code, string message, object? details = null) => new(ErrorKind.Validation, code, message, details);
    public static AppException Forbidden(string message = "Bạn không có quyền thực hiện thao tác này") => new(ErrorKind.Forbidden, "FORBIDDEN", message);
    public static AppException Unauthorized(string message = "Phiên đăng nhập không hợp lệ") => new(ErrorKind.Unauthorized, "UNAUTHORIZED", message);
}
