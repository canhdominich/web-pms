using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;

namespace EIM.Application.Features.Finance;

public sealed class FeePlanQuery : PageQuery
{
    public Guid? ProgramId { get; init; }
}

public sealed record FeePlanDto(
    Guid Id,
    Guid ProgramId,
    string ProgramCode,
    string Name,
    int Amount,
    string Currency,
    int SessionsPerWeek,
    bool IsDefault,
    DateTime CreatedAt);

public record UpdateFeePlanRequest
{
    [Required, MaxLength(255)]
    public string Name { get; init; } = "";

    [Range(1, int.MaxValue)]
    public int Amount { get; init; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = "VND";

    [Range(1, 7)]
    public int SessionsPerWeek { get; init; } = 2;

    public bool IsDefault { get; init; }
}

public sealed record CreateFeePlanRequest : UpdateFeePlanRequest
{
    public required Guid ProgramId { get; init; }
}

public sealed class InvoiceQuery : PageQuery
{
    public string? Status { get; init; }
    public Guid? StudentId { get; init; }
    public Guid? ClassId { get; init; }
    public Guid? EnrollmentId { get; init; }
    public DateOnly? DueFrom { get; init; }
    public DateOnly? DueTo { get; init; }
}

public sealed record InvoiceDto(
    Guid Id,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid ClassId,
    string ClassCode,
    int Amount,
    int PaidAmount,
    int Outstanding,
    string Status,
    DateOnly DueDate,
    DateTime? IssuedAt,
    DateTime CreatedAt);

public sealed record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    Guid StudentId,
    string StudentName,
    string ClassCode,
    int Amount,
    string Method,
    DateTime PaidAt,
    DateTime CreatedAt);

public sealed record InvoiceDetailDto(InvoiceDto Invoice, IReadOnlyList<PaymentDto> Payments);

public sealed record CreateInvoiceRequest
{
    public required Guid EnrollmentId { get; init; }

    [Range(1, int.MaxValue)]
    public int? Amount { get; init; }

    public required DateOnly DueDate { get; init; }

    public bool Issue { get; init; }
}

public sealed record UpdateInvoiceRequest
{
    [Range(1, int.MaxValue)]
    public int Amount { get; init; }

    public required DateOnly DueDate { get; init; }
}

public sealed record RecordPaymentRequest
{
    [Range(1, int.MaxValue)]
    public int Amount { get; init; }

    [Required, MaxLength(20)]
    public string Method { get; init; } = "";

    public DateTime? PaidAt { get; init; }
}

public sealed record ReceiptDto(
    string ReceiptNo,
    Guid PaymentId,
    DateTime PaidAt,
    int Amount,
    string Method,
    Guid InvoiceId,
    int InvoiceAmount,
    int TotalPaid,
    int Outstanding,
    string InvoiceStatus,
    Guid StudentId,
    string StudentName,
    string? GuardianName,
    string ClassCode,
    string ClassName);

public sealed class PaymentQuery : PageQuery
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public string? Method { get; init; }
    public Guid? StudentId { get; init; }
}

public sealed class PaymentStatusQuery : PageQuery
{
    public Guid? ClassId { get; init; }
    public string? State { get; init; }
    public string? Search { get; init; }
}

public sealed record PaymentStatusDto(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid ClassId,
    string ClassCode,
    string EnrollmentStatus,
    int TotalInvoiced,
    int TotalPaid,
    int Outstanding,
    int OverdueInvoices,
    string State);

public sealed record EnrollmentFinanceDto(
    Guid EnrollmentId,
    Guid ClassId,
    string ClassCode,
    string ClassName,
    string Status,
    int TotalInvoiced,
    int TotalPaid,
    int Outstanding,
    IReadOnlyList<InvoiceDetailDto> Invoices);

public sealed record StudentFinanceDto(
    Guid StudentId,
    string FullName,
    long TotalInvoiced,
    long TotalPaid,
    long Outstanding,
    IReadOnlyList<EnrollmentFinanceDto> Enrollments);

public sealed class FinanceExportQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public Guid? ClassId { get; init; }
}
