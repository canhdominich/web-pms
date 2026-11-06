using EIM.Application.Abstractions;
using EIM.Application.Features.Platform;
using EIM.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Finance;

public interface IOverdueInvoiceProcessor
{
    Task<int> ProcessAsync(CancellationToken ct);
}

internal sealed class OverdueInvoiceProcessor(IAppDbContext db, IAuditTrail audit, NotificationPublisher notifications, IClock clock) : IOverdueInvoiceProcessor
{
    public async Task<int> ProcessAsync(CancellationToken ct)
    {
        var today = clock.Today;
        var invoices = await db.Invoices
            .Include(i => i.Enrollment).ThenInclude(e => e.Student)
            .Include(i => i.Enrollment).ThenInclude(e => e.Classroom)
            .Where(i => i.Status == InvoiceStatuses.Issued && i.DueDate < today)
            .ToListAsync(ct);
        if (invoices.Count == 0) return 0;

        foreach (var invoice in invoices)
        {
            invoice.Status = InvoiceStatuses.Overdue;
            audit.Record(AuditActions.StatusChange, AuditEntities.Invoice, invoice.Id,
                new { From = InvoiceStatuses.Issued, To = InvoiceStatuses.Overdue, invoice.DueDate, Source = "system" });
            await notifications.PublishToRoleAsync(RoleCodes.Accountant, "Hóa đơn quá hạn",
                $"Hóa đơn {invoice.Id} của học viên {invoice.Enrollment.Student.FullName} (lớp {invoice.Enrollment.Classroom.Code}) " +
                $"số tiền {invoice.Amount:N0} đã quá hạn thanh toán ngày {invoice.DueDate:dd/MM/yyyy}.", ct);
        }
        await db.SaveChangesAsync(ct);
        return invoices.Count;
    }
}
