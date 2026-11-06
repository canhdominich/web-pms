using EIM.Api.Infrastructure;
using EIM.Application.Common;
using EIM.Application.Features.Users;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api/users"), Authorize(Roles = RoleSets.Root)]
public sealed class UsersController(IUserService users) : ApiControllerBase
{
    [HttpGet]
    public Task<PagedResult<UserDto>> List([FromQuery] UserQuery query, CancellationToken ct) => users.ListAsync(query, ct);

    [HttpGet("{id:guid}")]
    public Task<UserDto> Get(Guid id, CancellationToken ct) => users.GetAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request, CancellationToken ct)
    {
        var user = await users.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public Task<UserDto> Update(Guid id, UpdateUserRequest request, CancellationToken ct) => users.UpdateAsync(id, request, ct);

    [HttpPost("{id:guid}/roles")]
    public Task<UserDto> AssignRole(Guid id, AssignRoleRequest request, CancellationToken ct) => users.AssignRoleAsync(id, request, ct);

    [HttpDelete("{id:guid}/roles/{roleCode}")]
    public Task<UserDto> RevokeRole(Guid id, string roleCode, CancellationToken ct) => users.RevokeRoleAsync(id, roleCode, ct);
}

[Route("api/teachers"), Authorize(Roles = RoleSets.Staff)]
public sealed class TeachersController(IUserService users) : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<TeacherDto>> List(CancellationToken ct) => users.ListTeachersAsync(ct);
}

[Route("api/roles"), Authorize(Roles = RoleSets.Root)]
public sealed class RolesController(IUserService users) : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<RoleDto>> List(CancellationToken ct) => users.ListRolesAsync(ct);
}
