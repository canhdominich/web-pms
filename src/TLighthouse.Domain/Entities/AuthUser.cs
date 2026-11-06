using TLighthouse.Domain.Constants;

namespace TLighthouse.Domain.Entities;

public class AuthUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Status { get; set; } = UserStatuses.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
