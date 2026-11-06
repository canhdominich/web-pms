using EIM.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EIM.Api.Infrastructure;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, code, message, details) = exception switch
        {
            AppException app => (StatusOf(app.Kind), app.Code, app.Message, app.Details),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "CONCURRENCY_CONFLICT", "Dữ liệu đã bị thay đổi bởi thao tác khác", null),
            DbUpdateException => (StatusCodes.Status409Conflict, "DATA_CONSTRAINT", "Dữ liệu vi phạm ràng buộc của hệ thống", null),
            BadHttpRequestException bad => (bad.StatusCode, "BAD_REQUEST", "Yêu cầu không hợp lệ", null),
            OperationCanceledException => (499, "REQUEST_CANCELED", "Yêu cầu đã bị hủy", null),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Đã có lỗi xảy ra, vui lòng thử lại sau", (object?)null)
        };

        if (status >= StatusCodes.Status500InternalServerError) logger.LogError(exception, "Unhandled exception");
        else if (exception is DbUpdateException) logger.LogWarning(exception, "Database constraint violation");

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ErrorResponse(code, message, details, context.TraceIdentifier), ct);
        return true;
    }

    private static int StatusOf(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => StatusCodes.Status400BadRequest,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status409Conflict
    };
}
