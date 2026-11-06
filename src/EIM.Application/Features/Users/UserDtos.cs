using System.ComponentModel.DataAnnotations;
using EIM.Application.Common;

namespace EIM.Application.Features.Users;

public sealed class UserQuery : PageQuery
{
    public string? Search { get; init; }
    public string? Role { get; init; }
    public string? Status { get; init; }
}

public sealed record UserDto(Guid Id, string Email, string FullName, string Status, DateTime CreatedAt, IReadOnlyList<string> Roles);

public sealed record TeacherDto(Guid Id, string FullName, string Email);

public sealed record RoleDto(Guid Id, string Code, string Name, int UserCount);

public sealed record CreateUserRequest
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; init; } = "";

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; init; } = "";

    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [Required, MinLength(1)]
    public IReadOnlyList<string> Roles { get; init; } = [];
}

public sealed record UpdateUserRequest
{
    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [Required, MaxLength(50)]
    public string Status { get; init; } = "";

    [MinLength(8), MaxLength(128)]
    public string? Password { get; init; }
}

public sealed record AssignRoleRequest
{
    [Required, MaxLength(100)]
    public string RoleCode { get; init; } = "";
}
