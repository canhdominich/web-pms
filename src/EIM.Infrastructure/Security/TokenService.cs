using System.Globalization;
using System.Security.Claims;
using System.Text;
using EIM.Application.Abstractions;
using EIM.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EIM.Infrastructure.Security;

public sealed class TokenService(IOptions<JwtOptions> options, IClock clock) : ITokenService
{
    public const string RoleClaim = "role";
    public const string NameClaim = "name";
    private const string IssuedAtMsClaim = "iat_ms";

    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public static SymmetricSecurityKey CreateKey(string signingKey) => new(Encoding.UTF8.GetBytes(signingKey));

    public TokenPair Issue(AuthUser user, IReadOnlyCollection<string> roles)
    {
        var now = clock.UtcNow;
        var credentials = new SigningCredentials(CreateKey(_options.SigningKey), SecurityAlgorithms.HmacSha256);
        var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpires = now.AddDays(_options.RefreshTokenDays);

        var access = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = accessExpires,
            SigningCredentials = credentials,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                [JwtRegisteredClaimNames.Email] = user.Email,
                [NameClaim] = user.FullName,
                [RoleClaim] = roles.ToArray(),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
            }
        });

        var refresh = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.RefreshAudience,
            IssuedAt = now,
            NotBefore = now,
            Expires = refreshExpires,
            SigningCredentials = credentials,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                [IssuedAtMsClaim] = new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
            }
        });

        return new TokenPair(access, accessExpires, refresh, refreshExpires);
    }

    public async Task<RefreshTokenPayload?> ReadRefreshTokenAsync(string token)
    {
        var result = await _handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.RefreshAudience,
            IssuerSigningKey = CreateKey(_options.SigningKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        });
        if (!result.IsValid) return null;

        var identity = result.ClaimsIdentity;
        var sub = identity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var issued = identity.FindFirst(IssuedAtMsClaim)?.Value;
        return Guid.TryParse(sub, out var userId) && long.TryParse(issued, NumberStyles.None, CultureInfo.InvariantCulture, out var ms)
            ? new RefreshTokenPayload(userId, DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime)
            : null;
    }
}
