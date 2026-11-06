namespace TLighthouse.Domain.Entities;

public class AuthRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
