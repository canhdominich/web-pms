using System.Linq.Expressions;
using EIM.Domain.Constants;
using EIM.Domain.Entities;

namespace EIM.Application.Features.Finance;

internal static class FinanceProjections
{
    public const string StatePaid = "PAID";
    public const string StateUnpaid = "UNPAID";
    public const string StateNoInvoice = "NO_INVOICE";
    public static readonly string[] States = [StatePaid, StateUnpaid, StateNoInvoice];

    public static readonly Expression<Func<Invoice, InvoiceDto>> Invoice = i => new InvoiceDto(
        i.Id, i.EnrollmentId, i.Enrollment.StudentId, i.Enrollment.Student.FullName, i.Enrollment.ClassId, i.Enrollment.Classroom.Code,
        i.Amount, i.Payments.Sum(p => (int?)p.Amount) ?? 0, i.Amount - (i.Payments.Sum(p => (int?)p.Amount) ?? 0),
        i.Status, i.DueDate, i.IssuedAt, i.CreatedAt);

    public static readonly Expression<Func<Payment, PaymentDto>> Payment = p => new PaymentDto(
        p.Id, p.InvoiceId, p.Invoice.Enrollment.StudentId, p.Invoice.Enrollment.Student.FullName, p.Invoice.Enrollment.Classroom.Code,
        p.Amount, p.Method, p.PaidAt, p.CreatedAt);

    public static readonly Expression<Func<FeePlan, FeePlanDto>> FeePlan = f => new FeePlanDto(
        f.Id, f.ProgramId, f.Program.Code, f.Name, f.Amount, f.Currency, f.SessionsPerWeek, f.Program.FeePlanId == f.Id, f.CreatedAt);

    public static IQueryable<PaymentStatusRow> PaymentStatuses(this IQueryable<Enrollment> enrollments) =>
        enrollments.Select(e => new PaymentStatusRow
        {
            EnrollmentId = e.Id,
            StudentId = e.StudentId,
            StudentName = e.Student.FullName,
            ClassId = e.ClassId,
            ClassCode = e.Classroom.Code,
            EnrollmentStatus = e.Status,
            Invoiced = e.Invoices.Where(i => InvoiceStatuses.Billed.Contains(i.Status)).Sum(i => (int?)i.Amount) ?? 0,
            Paid = e.Invoices.Where(i => InvoiceStatuses.Billed.Contains(i.Status)).SelectMany(i => i.Payments).Sum(p => (int?)p.Amount) ?? 0,
            Overdue = e.Invoices.Count(i => i.Status == InvoiceStatuses.Overdue)
        });

    public static IQueryable<PaymentStatusRow> WhereState(this IQueryable<PaymentStatusRow> rows, string state) => state switch
    {
        StatePaid => rows.Where(r => r.Invoiced > 0 && r.Paid >= r.Invoiced),
        StateUnpaid => rows.Where(r => r.Paid < r.Invoiced),
        _ => rows.Where(r => r.Invoiced == 0)
    };

    public static PaymentStatusDto ToDto(this PaymentStatusRow r) => new(
        r.EnrollmentId, r.StudentId, r.StudentName, r.ClassId, r.ClassCode, r.EnrollmentStatus, r.Invoiced, r.Paid, r.Invoiced - r.Paid, r.Overdue,
        r.Invoiced == 0 ? StateNoInvoice : r.Paid >= r.Invoiced ? StatePaid : StateUnpaid);
}

internal sealed class PaymentStatusRow
{
    public Guid EnrollmentId { get; init; }
    public Guid StudentId { get; init; }
    public string StudentName { get; init; } = "";
    public Guid ClassId { get; init; }
    public string ClassCode { get; init; } = "";
    public string EnrollmentStatus { get; init; } = "";
    public int Invoiced { get; init; }
    public int Paid { get; init; }
    public int Overdue { get; init; }
}
