using EIM.Api.Infrastructure;
using EIM.Application.Common;
using EIM.Application.Features.Classes;
using EIM.Application.Features.Enrollments;
using EIM.Application.Features.Sessions;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api/classes"), Authorize(Roles = RoleSets.Staff)]
public sealed class ClassesController(IClassService classes) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<ClassDto>> List([FromQuery] ClassQuery query, CancellationToken ct) => classes.ListAsync(query, ct);

    [HttpGet("{id:guid}")]
    public Task<ClassDetailDto> Get(Guid id, CancellationToken ct) => classes.GetAsync(id, ct);

    [HttpPost, Authorize(Roles = RoleSets.AcademicEditors)]
    public async Task<ActionResult<ClassDetailDto>> Create(CreateClassRequest request, CancellationToken ct)
    {
        var created = await classes.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Class.Id }, created);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<ClassDto> Update(Guid id, UpdateClassRequest request, CancellationToken ct) => classes.UpdateAsync(id, request, ct);

    [HttpPost("{id:guid}/close"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<ClassDto> Close(Guid id, CancellationToken ct) => classes.CloseAsync(id, ct);

    [HttpGet("{id:guid}/students")]
    public Task<IReadOnlyList<RosterItemDto>> Roster(Guid id, CancellationToken ct) => classes.GetRosterAsync(id, ct);

    [HttpPost("{id:guid}/students"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<EnrollmentDto> AddStudent(Guid id, AddStudentRequest request, CancellationToken ct) => classes.AddStudentAsync(id, request, ct);

    [HttpPost("{id:guid}/promotion"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<PromotionResultDto> Promote(Guid id, PromotionRequest request, CancellationToken ct) => classes.PromoteAsync(id, request, ct);
}

[Route("api/classes/{classId:guid}"), Authorize(Roles = RoleSets.Staff)]
public sealed class ClassScheduleController(IClassScheduleService schedules, ISessionService sessions) : ApiControllerBase
{
    [HttpGet("schedule")]
    public Task<ScheduleDto> Get(Guid classId, CancellationToken ct) => schedules.GetAsync(classId, ct);

    [HttpPut("schedule"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<ScheduleDto> Reschedule(Guid classId, RescheduleRequest request, CancellationToken ct) => schedules.RescheduleAsync(classId, request, ct);

    [HttpPost("sessions/generate"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<ScheduleDto> Generate(Guid classId, GenerateSessionsRequest request, CancellationToken ct) => schedules.GenerateAsync(classId, request, ct);

    [HttpGet("sessions")]
    public Task<IReadOnlyList<SessionDto>> Sessions(Guid classId, [FromQuery] SessionQuery query, CancellationToken ct) =>
        sessions.ListByClassAsync(classId, query, ct);
}

[Route("api/classes/{classId:guid}/teachers"), Authorize(Roles = RoleSets.Staff)]
public sealed class ClassStaffController(IClassStaffService staff) : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ClassTeacherDto>> List(Guid classId, CancellationToken ct) => staff.ListAsync(classId, ct);

    [HttpPost, Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<StaffChangeResultDto> Assign(Guid classId, AssignTeacherRequest request, CancellationToken ct) => staff.AssignAsync(classId, request, ct);

    [HttpDelete("{teacherId:guid}"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<StaffChangeResultDto> Unassign(Guid classId, Guid teacherId, [FromQuery] DateOnly? fromDate, CancellationToken ct) =>
        staff.UnassignAsync(classId, teacherId, fromDate, ct);

    [HttpPut("main"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<StaffChangeResultDto> ChangeMain(Guid classId, ChangeMainTeacherRequest request, CancellationToken ct) =>
        staff.ChangeMainAsync(classId, request, ct);
}
