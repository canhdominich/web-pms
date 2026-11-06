using TLighthouse.Api.Infrastructure;
using TLighthouse.Application.Features.Sessions;
using TLighthouse.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TLighthouse.Api.Controllers;

[Route("api"), Authorize]
public sealed class SessionsController(ISessionService sessions) : ApiControllerBase
{
    [HttpGet("sessions/my"), Authorize(Roles = RoleSets.Teachers)]
    public Task<IReadOnlyList<SessionDto>> Mine([FromQuery] SessionQuery query, CancellationToken ct) => sessions.ListMineAsync(query, ct);

    [HttpGet("teachers/{teacherId:guid}/sessions"), Authorize(Roles = RoleSets.Staff)]
    public Task<IReadOnlyList<SessionDto>> ByTeacher(Guid teacherId, [FromQuery] SessionQuery query, CancellationToken ct) =>
        sessions.ListByTeacherAsync(teacherId, query, ct);

    [HttpGet("sessions/{id:guid}")]
    public Task<SessionDetailDto> Get(Guid id, CancellationToken ct) => sessions.GetAsync(id, ct);

    [HttpPut("sessions/{id:guid}"), Authorize(Roles = RoleSets.AcademicEditors)]
    public Task<SessionDto> Update(Guid id, UpdateSessionRequest request, CancellationToken ct) => sessions.UpdateAsync(id, request, ct);
}
