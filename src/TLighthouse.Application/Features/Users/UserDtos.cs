using System.ComponentModel.DataAnnotations;
using TLighthouse.Application.Common;

namespace TLighthouse.Application.Features.Users;

public sealed class UserQuery : PageQuery
{
    public string? Search { get; init; }
    public string? Role { get; init; }
    public string? Status { get; init; }
}

public sealed record UserDto(Guid Id, string Email, string FullName, string Status, DateTime CreatedAt, string Role);

public sealed record TeacherDto(Guid Id, string FullName, string Email);

public sealed record RoleDto(string Code, string Name, int UserCount);

public sealed record CreateUserRequest
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; init; } = "";

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; init; } = "";

    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [Required, MaxLength(50)]
    public string Role { get; init; } = "";
}

public sealed record UpdateUserRequest
{
    [Required, MaxLength(255)]
    public string FullName { get; init; } = "";

    [Required, MaxLength(50)]
    public string Status { get; init; } = "";

    [Required, MaxLength(50)]
    public string Role { get; init; } = "";

    [MinLength(8), MaxLength(128)]
    public string? Password { get; init; }
}
