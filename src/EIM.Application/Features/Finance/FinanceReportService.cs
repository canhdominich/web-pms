using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Finance;

public interface IFinanceReportService
{
    Task<PagedResult<PaymentStatusDto>> GetPaymentStatusAsync(PaymentStatusQuery query, CancellationToken ct);
    Task<StudentFinanceDto> GetStudentFinanceAsync(Guid studentId, CancellationToken ct);
    Task<FileContent> ExportAsync(FinanceExportQuery query, CancellationToken ct);
}

internal sealed class FinanceReportService(IAppDbContext db, IExcelService excel, IAuditTrail audit, IClock clock) : IFinanceReportService
{
    public async Task<PagedResult<PaymentStatusDto>> GetPaymentStatusAsync(PaymentStatusQuery query, CancellationToken ct)
    {
        var enrollments = db.Enrollments.AsNoTracking().Where(e => EnrollmentStatuses.Occupying.Contains(e.Status));
        if (query.ClassId is { } classId) enrollments = enrollments.Where(e => e.ClassId == classId);
        if (query.Search.Clean() is { } search) enrollments = enrollments.Where(e => e.Student.FullName.Contains(search));

        var rows = enrollments.PaymentStatuses();
        if (query.State.Clean() is { } state)
            rows = rows.WhereState(Guard.OneOf(state, FinanceProjections.States, "Tình trạng"));
        var page = await rows.OrderByDescending(r => r.Invoiced - r.Paid).ThenBy(r => r.StudentName).ToPagedAsync(query, ct);
        return new PagedResult<PaymentStatusDto>(page.Items.Select(r => r.ToDto()).ToList(), page.Page, page.PageSize, page.Total);
    }

    public async Task<StudentFinanceDto> GetStudentFinanceAsync(Guid studentId, CancellationToken ct)
    {
        var student = await db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studentId, ct) ?? throw AppException.NotFound("học viên");
        var enrollments = await db.Enrollments.AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.StartDate)
            .Select(e => new { e.Id, e.ClassId, e.Classroom.Code, e.Classroom.Name, e.Status })
            .ToListAsync(ct);
        var invoices = await db.Invoices.AsNoTracking().Where(i => i.Enrollment.StudentId == studentId)
            .OrderBy(i => i.DueDate).Select(FinanceProjections.Invoice).ToListAsync(ct);
        var payments = await db.Payments.AsNoTracking().Where(p => p.Invoice.Enrollment.StudentId == studentId)
            .OrderBy(p => p.PaidAt).Select(FinanceProjections.Payment).ToListAsync(ct);

        var items = enrollments.Select(e =>
        {
            var own = invoices.Where(i => i.EnrollmentId == e.Id).ToList();
            var billed = own.Where(i => InvoiceStatuses.Billed.Contains(i.Status)).ToList();
            return new EnrollmentFinanceDto(e.Id, e.ClassId, e.Code, e.Name, e.Status,
                billed.Sum(i => i.Amount), billed.Sum(i => i.PaidAmount), billed.Sum(i => i.Outstanding),
                own.Select(i => new InvoiceDetailDto(i, payments.Where(p => p.InvoiceId == i.Id).ToList())).ToList());
        }).ToList();

        return new StudentFinanceDto(student.Id, student.FullName,
            items.Sum(i => (long)i.TotalInvoiced), items.Sum(i => (long)i.TotalPaid), items.Sum(i => (long)i.Outstanding), items);
    }

    public async Task<FileContent> ExportAsync(FinanceExportQuery query, CancellationToken ct)
    {
        var invoices = db.Invoices.AsNoTracking();
        var payments = db.Payments.AsNoTracking();
        var enrollments = db.Enrollments.AsNoTracking().Where(e => e.Invoices.Any());
        if (query.From is { } from)
        {
            var fromUtc = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            invoices = invoices.Where(i => i.DueDate >= from);
            payments = payments.Where(p => p.PaidAt >= fromUtc);
        }
        if (query.To is { } to)
        {
            var toUtc = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            invoices = invoices.Where(i => i.DueDate <= to);
            payments = payments.Where(p => p.PaidAt < toUtc);
        }
        if (query.ClassId is { } classId)
        {
            invoices = invoices.Where(i => i.Enrollment.ClassId == classId);
            payments = payments.Where(p => p.Invoice.Enrollment.ClassId == classId);
            enrollments = enrollments.Where(e => e.ClassId == classId);
        }

        var invoiceRows = await invoices.OrderBy(i => i.DueDate).Select(FinanceProjections.Invoice).ToListAsync(ct);
        var paymentRows = await payments.OrderBy(p => p.PaidAt).Select(FinanceProjections.Payment).ToListAsync(ct);
        var statusRows = (await enrollments.PaymentStatuses().OrderBy(r => r.ClassCode).ThenBy(r => r.StudentName).ToListAsync(ct))
            .Select(r => r.ToDto())
            .ToList();

        var invoiceSheet = new ExcelSheet("Invoices",
            ["invoice_id", "student", "class_code", "amount", "paid", "outstanding", "status", "due_date", "issued_at", "created_at"],
            invoiceRows.Select(i => (IReadOnlyList<object?>)
                [i.Id, i.StudentName, i.ClassCode, i.Amount, i.PaidAmount, i.Outstanding, i.Status, i.DueDate, i.IssuedAt, i.CreatedAt]));
        var paymentSheet = new ExcelSheet("Payments",
            ["payment_id", "invoice_id", "student", "class_code", "amount", "method", "paid_at"],
            paymentRows.Select(p => (IReadOnlyList<object?>)[p.Id, p.InvoiceId, p.StudentName, p.ClassCode, p.Amount, p.Method, p.PaidAt]));
        var statusSheet = new ExcelSheet("PaymentStatus",
            ["enrollment_id", "student", "class_code", "enrollment_status", "invoiced", "paid", "outstanding", "overdue_invoices", "state"],
            statusRows.Select(s => (IReadOnlyList<object?>)
                [s.EnrollmentId, s.StudentName, s.ClassCode, s.EnrollmentStatus, s.TotalInvoiced, s.TotalPaid, s.Outstanding, s.OverdueInvoices, s.State]));

        audit.Record(AuditActions.Export, AuditEntities.Invoice, null, new
        {
            query.From,
            query.To,
            query.ClassId,
            Invoices = invoiceRows.Count,
            Payments = paymentRows.Count
        });
        await db.SaveChangesAsync(ct);
        return new FileContent(excel.Write(invoiceSheet, paymentSheet, statusSheet), $"finance-report-{clock.Today:yyyyMMdd}.xlsx");
    }
}
