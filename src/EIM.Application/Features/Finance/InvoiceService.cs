using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Finance;

public interface IInvoiceService
{
    Task<PagedResult<InvoiceDto>> ListAsync(InvoiceQuery query, CancellationToken ct);
    Task<InvoiceDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<InvoiceDetailDto> CreateAsync(CreateInvoiceRequest request, CancellationToken ct);
    Task<InvoiceDetailDto> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken ct);
    Task<InvoiceDetailDto> IssueAsync(Guid id, CancellationToken ct);
    Task<InvoiceDetailDto> CancelAsync(Guid id, CancellationToken ct);
    Task<ReceiptDto> RecordPaymentAsync(Guid id, RecordPaymentRequest request, CancellationToken ct);
    Task<PagedResult<PaymentDto>> ListPaymentsAsync(PaymentQuery query, CancellationToken ct);
    Task<ReceiptDto> GetReceiptAsync(Guid paymentId, CancellationToken ct);
}

internal sealed class InvoiceService(IAppDbContext db, IAuditTrail audit, IClock clock) : IInvoiceService
{
    public Task<PagedResult<InvoiceDto>> ListAsync(InvoiceQuery query, CancellationToken ct)
    {
        var invoices = db.Invoices.AsNoTracking();
        if (query.Status.Clean() is { } status) invoices = invoices.Where(i => i.Status == status.ToUpper());
        if (query.StudentId is { } studentId) invoices = invoices.Where(i => i.Enrollment.StudentId == studentId);
        if (query.ClassId is { } classId) invoices = invoices.Where(i => i.Enrollment.ClassId == classId);
        if (query.EnrollmentId is { } enrollmentId) invoices = invoices.Where(i => i.EnrollmentId == enrollmentId);
        if (query.DueFrom is { } from) invoices = invoices.Where(i => i.DueDate >= from);
        if (query.DueTo is { } to) invoices = invoices.Where(i => i.DueDate <= to);
        return invoices.OrderBy(i => i.DueDate).ThenBy(i => i.CreatedAt).Select(FinanceProjections.Invoice).ToPagedAsync(query, ct);
    }

    public async Task<InvoiceDetailDto> GetAsync(Guid id, CancellationToken ct)
    {
        var invoice = await db.Invoices.AsNoTracking().Where(i => i.Id == id).Select(FinanceProjections.Invoice).FirstOrDefaultAsync(ct)
                      ?? throw AppException.NotFound("hóa đơn");
        var payments = await db.Payments.AsNoTracking().Where(p => p.InvoiceId == id).OrderBy(p => p.PaidAt)
            .Select(FinanceProjections.Payment).ToListAsync(ct);
        return new InvoiceDetailDto(invoice, payments);
    }

    public async Task<InvoiceDetailDto> CreateAsync(CreateInvoiceRequest request, CancellationToken ct)
    {
        var enrollment = await db.Enrollments.AsNoTracking()
            .Where(e => e.Id == request.EnrollmentId)
            .Select(e => new
            {
                e.Status,
                DefaultAmount = e.Classroom.Program.FeePlans.Where(f => f.Id == e.Classroom.Program.FeePlanId).Select(f => (int?)f.Amount).FirstOrDefault()
            })
            .FirstOrDefaultAsync(ct) ?? throw AppException.NotFound("ghi danh");
        if (!EnrollmentStatuses.Occupying.Contains(enrollment.Status))
            throw AppException.Conflict("ENROLLMENT_NOT_BILLABLE", "Chỉ lập hóa đơn cho ghi danh đang học hoặc bảo lưu");

        var amount = request.Amount ?? enrollment.DefaultAmount
                     ?? throw AppException.Invalid("AMOUNT_REQUIRED", "Chương trình chưa có gói học phí mặc định, cần nhập số tiền");
        var invoice = new Invoice
        {
            EnrollmentId = request.EnrollmentId,
            Amount = amount,
            DueDate = request.DueDate,
            Status = InvoiceStatuses.Draft,
            CreatedAt = clock.UtcNow
        };
        if (request.Issue) MarkIssued(invoice);
        db.Invoices.Add(invoice);
        audit.Record(AuditActions.Create, AuditEntities.Invoice, invoice.Id, Snapshot(invoice));
        await db.SaveChangesAsync(ct);
        return await GetAsync(invoice.Id, ct);
    }

    public async Task<InvoiceDetailDto> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken ct)
    {
        var invoice = await FindAsync(id, ct);
        if (invoice.Status != InvoiceStatuses.Draft)
            throw AppException.Conflict("INVOICE_LOCKED", "Chỉ được sửa hóa đơn ở trạng thái DRAFT");
        var before = Snapshot(invoice);
        invoice.Amount = request.Amount;
        invoice.DueDate = request.DueDate;
        audit.Record(AuditActions.Update, AuditEntities.Invoice, id, new { Before = before, After = Snapshot(invoice) });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<InvoiceDetailDto> IssueAsync(Guid id, CancellationToken ct)
    {
        var invoice = await FindAsync(id, ct);
        if (invoice.Status != InvoiceStatuses.Draft)
            throw AppException.Conflict("INVALID_TRANSITION", "Chỉ phát hành hóa đơn ở trạng thái DRAFT");
        MarkIssued(invoice);
        audit.Record(AuditActions.Issue, AuditEntities.Invoice, id, new { From = InvoiceStatuses.Draft, To = invoice.Status, invoice.IssuedAt });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<InvoiceDetailDto> CancelAsync(Guid id, CancellationToken ct)
    {
        var invoice = await FindAsync(id, ct);
        if (invoice.Status is InvoiceStatuses.Paid or InvoiceStatuses.Canceled)
            throw AppException.Conflict("INVALID_TRANSITION", "Không thể hủy hóa đơn đã thanh toán hoặc đã hủy");
        if (await db.Payments.AnyAsync(p => p.InvoiceId == id, ct))
            throw AppException.Conflict("INVOICE_HAS_PAYMENTS", "Hóa đơn đã có thanh toán, không thể hủy");
        var from = invoice.Status;
        invoice.Status = InvoiceStatuses.Canceled;
        audit.Record(AuditActions.Cancel, AuditEntities.Invoice, id, new { From = from, To = invoice.Status });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public Task<ReceiptDto> RecordPaymentAsync(Guid id, RecordPaymentRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var invoice = await db.LockInvoiceAsync(id, ct) ?? throw AppException.NotFound("hóa đơn");
            if (!InvoiceStatuses.Payable.Contains(invoice.Status))
                throw AppException.Conflict("INVOICE_NOT_PAYABLE", "Hóa đơn không ở trạng thái có thể thanh toán");

            var method = Guard.OneOf(request.Method, PaymentMethods.All, "Phương thức thanh toán");
            var paidAt = request.PaidAt?.ToUniversalTime() ?? clock.UtcNow;
            if (paidAt > clock.UtcNow.AddMinutes(5))
                throw AppException.Invalid("INVALID_PAID_AT", "Thời điểm thanh toán không được ở tương lai");

            var paid = await db.Payments.Where(p => p.InvoiceId == id).SumAsync(p => (int?)p.Amount, ct) ?? 0;
            if (paid + request.Amount > invoice.Amount)
                throw AppException.Conflict("OVERPAYMENT", $"Số tiền vượt quá công nợ còn lại ({invoice.Amount - paid})");

            var payment = new Payment
            {
                InvoiceId = id,
                Amount = request.Amount,
                Method = method,
                PaidAt = paidAt,
                CreatedAt = clock.UtcNow
            };
            db.Payments.Add(payment);
            var from = invoice.Status;
            if (paid + request.Amount == invoice.Amount) invoice.Status = InvoiceStatuses.Paid;

            audit.Record(AuditActions.Payment, AuditEntities.Payment, payment.Id, new { InvoiceId = id, payment.Amount, payment.Method, payment.PaidAt });
            if (from != invoice.Status)
                audit.Record(AuditActions.StatusChange, AuditEntities.Invoice, id, new { From = from, To = invoice.Status, PaymentId = payment.Id });
            await db.SaveChangesAsync(ct);
            return await GetReceiptAsync(payment.Id, ct);
        }, ct);

    public Task<PagedResult<PaymentDto>> ListPaymentsAsync(PaymentQuery query, CancellationToken ct)
    {
        var payments = db.Payments.AsNoTracking();
        if (query.From is { } from) payments = payments.Where(p => p.PaidAt >= from.ToUniversalTime());
        if (query.To is { } to) payments = payments.Where(p => p.PaidAt <= to.ToUniversalTime());
        if (query.Method.Clean() is { } method) payments = payments.Where(p => p.Method == method.ToUpper());
        if (query.StudentId is { } studentId) payments = payments.Where(p => p.Invoice.Enrollment.StudentId == studentId);
        return payments.OrderByDescending(p => p.PaidAt).Select(FinanceProjections.Payment).ToPagedAsync(query, ct);
    }

    public async Task<ReceiptDto> GetReceiptAsync(Guid paymentId, CancellationToken ct)
    {
        var data = await db.Payments.AsNoTracking()
            .Where(p => p.Id == paymentId)
            .Select(p => new
            {
                Payment = p,
                p.Invoice.Amount,
                p.Invoice.Status,
                TotalPaid = p.Invoice.Payments.Where(x => x.PaidAt <= p.PaidAt).Sum(x => x.Amount),
                p.Invoice.Enrollment.StudentId,
                p.Invoice.Enrollment.Student.FullName,
                p.Invoice.Enrollment.Student.GuardianName,
                p.Invoice.Enrollment.Classroom.Code,
                ClassName = p.Invoice.Enrollment.Classroom.Name
            })
            .FirstOrDefaultAsync(ct) ?? throw AppException.NotFound("phiếu thu");

        var p = data.Payment;
        return new ReceiptDto(
            $"PT-{clock.LocalDate(p.PaidAt):yyyyMMdd}-{p.Id.ToString("N")[..8].ToUpperInvariant()}",
            p.Id, p.PaidAt, p.Amount, p.Method, p.InvoiceId, data.Amount, data.TotalPaid, data.Amount - data.TotalPaid, data.Status,
            data.StudentId, data.FullName, data.GuardianName, data.Code, data.ClassName);
    }

    private async Task<Invoice> FindAsync(Guid id, CancellationToken ct) =>
        await db.Invoices.FirstOrDefaultAsync(i => i.Id == id, ct) ?? throw AppException.NotFound("hóa đơn");

    private void MarkIssued(Invoice invoice)
    {
        invoice.IssuedAt = clock.UtcNow;
        invoice.Status = invoice.DueDate < clock.Today ? InvoiceStatuses.Overdue : InvoiceStatuses.Issued;
    }

    private static object Snapshot(Invoice i) => new { i.EnrollmentId, i.Amount, i.Status, i.DueDate, i.IssuedAt };
}
