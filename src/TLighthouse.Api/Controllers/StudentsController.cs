using TLighthouse.Api.Infrastructure;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Enrollments;
using TLighthouse.Application.Features.Students;
using TLighthouse.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TLighthouse.Api.Controllers;

[Route("api/students"), Authorize(Roles = RoleSets.Staff)]
public sealed class StudentsController(IStudentService students, IEnrollmentService enrollments) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<StudentDto>> List([FromQuery] StudentQuery query, CancellationToken ct) => students.ListAsync(query, ct);

    [HttpGet("{id:guid}")]
    public Task<StudentDto> Get(Guid id, CancellationToken ct) => students.GetAsync(id, ct);

    [HttpPost, Authorize(Roles = RoleSets.StudentEditors)]
    public async Task<ActionResult<StudentDto>> Create(SaveStudentRequest request, CancellationToken ct)
    {
        var student = await students.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = RoleSets.StudentEditors)]
    public Task<StudentDto> Update(Guid id, SaveStudentRequest request, CancellationToken ct) => students.UpdateAsync(id, request, ct);

    [HttpGet("export")]
    public async Task<FileContentResult> Export([FromQuery] StudentQuery query, CancellationToken ct) => Excel(await students.ExportAsync(query, ct));

    [HttpGet("{id:guid}/enrollments")]
    public Task<IReadOnlyList<EnrollmentDto>> Enrollments(Guid id, CancellationToken ct) => enrollments.ListByStudentAsync(id, ct);
}

[Route("api/enrollments"), Authorize(Roles = RoleSets.Staff)]
public sealed class EnrollmentsController(IEnrollmentService enrollments) : ApiControllerBase
{
    [HttpPost, Authorize(Roles = RoleSets.AcademicEditors)]
    public async Task<ActionResult<EnrollmentDto>> Create(CreateEnrollmentRequest request, CancellationToken ct)
    {
        var enrollment = await enrollments.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = enrollment.Id }, enrollment);
    }

    [HttpGet("{id:guid}")]
    public Task<EnrollmentDetailDto> Get(Guid id, CancellationToken ct) => enrollments.GetAsync(id, ct);

    [HttpPatch("{id:guid}/status"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<EnrollmentDto> ChangeStatus(Guid id, ChangeEnrollmentStatusRequest request, CancellationToken ct) =>
        enrollments.ChangeStatusAsync(id, request, ct);

    [HttpPost("{id:guid}/transfer"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<EnrollmentDetailDto> Transfer(Guid id, TransferEnrollmentRequest request, CancellationToken ct) =>
        enrollments.TransferAsync(id, request, ct);
}
