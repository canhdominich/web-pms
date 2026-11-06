using TLighthouse.Application.Common;

namespace TLighthouse.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsInRole(string role);

    Guid RequiredId => UserId ?? throw AppException.Unauthorized();

    bool IsInAnyRole(string roleSet) =>
        roleSet.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(IsInRole);
}
