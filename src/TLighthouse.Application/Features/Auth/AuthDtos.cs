using System.ComponentModel.DataAnnotations;
using TLighthouse.Application.Abstractions;

namespace TLighthouse.Application.Features.Auth;

public sealed record LoginRequest
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; init; } = "";

    [Required, MaxLength(128)]
    public string Password { get; init; } = "";
}

public sealed record RefreshRequest
{
    [Required]
    public string RefreshToken { get; init; } = "";
}

public sealed record CurrentUserDto(Guid Id, string Email, string FullName, string Status, IReadOnlyList<string> Roles);

public sealed record AuthResponse(TokenPair Tokens, CurrentUserDto User);
