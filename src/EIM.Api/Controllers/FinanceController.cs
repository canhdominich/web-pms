using EIM.Api.Infrastructure;
using EIM.Application.Common;
using EIM.Application.Features.Finance;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api/fee-plans"), Authorize]
public sealed class FeePlansController(IFeePlanService plans) : ApiControllerBase
{
    [HttpGet, Authorize(Roles = RoleSets.Staff)]
    public Task<PagedResult<FeePlanDto>> List([FromQuery] FeePlanQuery query, CancellationToken ct) => plans.ListAsync(query, ct);

    [HttpGet("{id:guid}"), Authorize(Roles = RoleSets.Staff)]
    public Task<FeePlanDto> Get(Guid id, CancellationToken ct) => plans.GetAsync(id, ct);

    [HttpPost, Authorize(Roles = RoleSets.FinanceEditors)]
    public async Task<ActionResult<FeePlanDto>> Create(CreateFeePlanRequest request, CancellationToken ct)
    {
        var plan = await plans.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = plan.Id }, plan);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = RoleSets.FinanceEditors)]
    public Task<FeePlanDto> Update(Guid id, UpdateFeePlanRequest request, CancellationToken ct) => plans.UpdateAsync(id, request, ct);

    [HttpDelete("{id:guid}"), Authorize(Roles = RoleSets.FinanceEditors)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await plans.DeleteAsync(id, ct);
        return NoContent();
    }
}

[Route("api/invoices"), Authorize(Roles = RoleSets.FinanceViewers)]
public sealed class InvoicesController(IInvoiceService invoices) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<InvoiceDto>> List([FromQuery] InvoiceQuery query, CancellationToken ct) => invoices.ListAsync(query, ct);

    [HttpGet("{id:guid}")]
    public Task<InvoiceDetailDto> Get(Guid id, CancellationToken ct) => invoices.GetAsync(id, ct);

    [HttpPost, Authorize(Roles = RoleSets.FinanceEditors)]
    public async Task<ActionResult<InvoiceDetailDto>> Create(CreateInvoiceRequest request, CancellationToken ct)
    {
        var invoice = await invoices.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = invoice.Invoice.Id }, invoice);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = RoleSets.FinanceEditors)]
    public Task<InvoiceDetailDto> Update(Guid id, UpdateInvoiceRequest request, CancellationToken ct) => invoices.UpdateAsync(id, request, ct);

    [HttpPost("{id:guid}/issue"), Authorize(Roles = RoleSets.FinanceEditors)]
    public Task<InvoiceDetailDto> Issue(Guid id, CancellationToken ct) => invoices.IssueAsync(id, ct);

    [HttpPost("{id:guid}/cancel"), Authorize(Roles = RoleSets.FinanceEditors)]
    public Task<InvoiceDetailDto> Cancel(Guid id, CancellationToken ct) => invoices.CancelAsync(id, ct);

    [HttpPost("{id:guid}/payments"), Authorize(Roles = RoleSets.FinanceEditors)]
    public Task<ReceiptDto> RecordPayment(Guid id, RecordPaymentRequest request, CancellationToken ct) => invoices.RecordPaymentAsync(id, request, ct);
}

[Route("api/payments"), Authorize(Roles = RoleSets.FinanceViewers)]
public sealed class PaymentsController(IInvoiceService invoices) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<PaymentDto>> List([FromQuery] PaymentQuery query, CancellationToken ct) => invoices.ListPaymentsAsync(query, ct);

    [HttpGet("{id:guid}/receipt")]
    public Task<ReceiptDto> Receipt(Guid id, CancellationToken ct) => invoices.GetReceiptAsync(id, ct);
}

[Route("api"), Authorize(Roles = RoleSets.FinanceViewers)]
public sealed class FinanceReportsController(IFinanceReportService reports) : ApiControllerBase
{
    [HttpGet("finance/payment-status")]
    public Task<PagedResult<PaymentStatusDto>> PaymentStatus([FromQuery] PaymentStatusQuery query, CancellationToken ct) =>
        reports.GetPaymentStatusAsync(query, ct);

    [HttpGet("students/{studentId:guid}/finance")]
    public Task<StudentFinanceDto> StudentFinance(Guid studentId, CancellationToken ct) => reports.GetStudentFinanceAsync(studentId, ct);

    [HttpGet("finance/export")]
    public async Task<FileContentResult> Export([FromQuery] FinanceExportQuery query, CancellationToken ct) => Excel(await reports.ExportAsync(query, ct));
}
