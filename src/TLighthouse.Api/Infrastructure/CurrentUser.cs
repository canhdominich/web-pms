using TLighthouse.Application.Abstractions;
using TLighthouse.Infrastructure.Security;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TLighthouse.Api.Infrastructure;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id) ? id : null;

    public bool IsInRole(string role) =>
        accessor.HttpContext?.User.HasClaim(TokenService.RoleClaim, role) ?? false;
}
