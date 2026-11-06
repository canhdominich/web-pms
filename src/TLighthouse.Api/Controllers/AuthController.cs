using TLighthouse.Api.Infrastructure;
using TLighthouse.Application.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TLighthouse.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ApiControllerBase
{
    [HttpPost("login"), AllowAnonymous, EnableRateLimiting(ServiceCollectionExtensions.AuthRateLimit)]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken ct) => auth.LoginAsync(request, ct);

    [HttpPost("refresh"), AllowAnonymous, EnableRateLimiting(ServiceCollectionExtensions.AuthRateLimit)]
    public Task<AuthResponse> Refresh(RefreshRequest request, CancellationToken ct) => auth.RefreshAsync(request, ct);

    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await auth.LogoutAsync(ct);
        return NoContent();
    }

    [HttpGet("me"), Authorize]
    public Task<CurrentUserDto> Me(CancellationToken ct) => auth.GetCurrentAsync(ct);
}
