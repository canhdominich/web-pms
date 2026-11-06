using TLighthouse.Api.Infrastructure;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Trials;
using TLighthouse.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TLighthouse.Api.Controllers;

[Route("api"), Authorize(Roles = RoleSets.TrialViewers)]
public sealed class TrialsController(ITrialService trials) : ApiControllerBase
{
    [HttpGet("trial-leads")]
    public Task<PagedResult<TrialLeadDto>> List([FromQuery] TrialLeadQuery query, CancellationToken ct) => trials.ListAsync(query, ct);

    [HttpGet("trial-leads/{id:guid}")]
    public Task<TrialLeadDetailDto> Get(Guid id, CancellationToken ct) => trials.GetAsync(id, ct);

    [HttpPost("trial-leads"), Authorize(Roles = RoleSets.TrialEditors)]
    public async Task<ActionResult<TrialLeadDto>> Create(SaveTrialLeadRequest request, CancellationToken ct)
    {
        var lead = await trials.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = lead.Id }, lead);
    }

    [HttpPut("trial-leads/{id:guid}"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<TrialLeadDto> Update(Guid id, SaveTrialLeadRequest request, CancellationToken ct) => trials.UpdateAsync(id, request, ct);

    [HttpPatch("trial-leads/{id:guid}/status"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<TrialLeadDto> ChangeStatus(Guid id, ChangeTrialStatusRequest request, CancellationToken ct) => trials.ChangeStatusAsync(id, request, ct);

    [HttpGet("trial-leads/export")]
    public async Task<FileContentResult> Export([FromQuery] TrialLeadQuery query, CancellationToken ct) => Excel(await trials.ExportAsync(query, ct));

    [HttpPost("trial-leads/{id:guid}/schedule"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<TrialLeadDetailDto> Schedule(Guid id, ScheduleTrialRequest request, CancellationToken ct) => trials.ScheduleAsync(id, request, ct);

    [HttpPatch("trial-leads/{id:guid}/schedule/result"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<TrialLeadDetailDto> RecordResult(Guid id, TrialResultRequest request, CancellationToken ct) => trials.RecordResultAsync(id, request, ct);

    [HttpPost("trial-leads/{id:guid}/schedule/cancel"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<TrialLeadDetailDto> CancelSchedule(Guid id, CancellationToken ct) => trials.CancelScheduleAsync(id, ct);

    [HttpPost("trial-leads/{id:guid}/convert"), Authorize(Roles = RoleSets.TrialEditors)]
    public Task<ConvertTrialResultDto> Convert(Guid id, ConvertTrialRequest request, CancellationToken ct) => trials.ConvertAsync(id, request, ct);
}
