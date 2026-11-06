using TLighthouse.Domain.Entities;

namespace TLighthouse.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public sealed record TokenPair(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt);

public sealed record RefreshTokenPayload(Guid UserId, DateTime IssuedAt);

public interface ITokenService
{
    TokenPair Issue(AuthUser user, IReadOnlyCollection<string> roles);
    Task<RefreshTokenPayload?> ReadRefreshTokenAsync(string token);
}
