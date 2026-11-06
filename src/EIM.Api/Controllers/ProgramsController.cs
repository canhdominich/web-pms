using EIM.Api.Infrastructure;
using EIM.Application.Common;
using EIM.Application.Features.Programs;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api/programs"), Authorize]
public sealed class ProgramsController(IProgramService programs) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<ProgramDto>> List([FromQuery] ProgramQuery query, CancellationToken ct) => programs.ListAsync(query, ct);

    [HttpGet("{id:guid}")]
    public Task<ProgramDetailDto> Get(Guid id, CancellationToken ct) => programs.GetAsync(id, ct);

    [HttpPost, Authorize(Roles = RoleSets.CurriculumEditors)]
    public async Task<ActionResult<ProgramDetailDto>> Create(CreateProgramRequest request, CancellationToken ct)
    {
        var program = await programs.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = program.Program.Id }, program);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = RoleSets.CurriculumEditors)]
    public Task<ProgramDetailDto> Update(Guid id, UpdateProgramRequest request, CancellationToken ct) => programs.UpdateAsync(id, request, ct);

    [HttpGet("{id:guid}/units")]
    public Task<ProgramStructureDto> GetUnits(Guid id, CancellationToken ct) => programs.GetUnitsAsync(id, ct);

    [HttpPut("{id:guid}/units"), Authorize(Roles = RoleSets.CurriculumEditors)]
    public Task<ProgramStructureDto> UpdateUnits(Guid id, UpdateUnitsRequest request, CancellationToken ct) => programs.UpdateUnitsAsync(id, request, ct);

    [HttpGet("export"), Authorize(Roles = RoleSets.Staff)]
    public async Task<FileContentResult> Export(CancellationToken ct) => Excel(await programs.ExportAsync(ct));

    [HttpPost("import"), Authorize(Roles = RoleSets.CurriculumEditors), RequestSizeLimit(MaxUploadBytes), Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        return Import(await programs.ImportAsync(stream, ct));
    }
}
