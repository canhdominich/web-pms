using System.ComponentModel.DataAnnotations;

namespace EIM.Infrastructure;

public sealed class DatabaseOptions
{
    public const string Section = "Database";

    [Required]
    public string ConnectionString { get; init; } = "";

    [Required]
    public string ServerVersion { get; init; } = "8.0.36";

    public bool CreateSchemaOnStartup { get; init; } = true;
}

public sealed class JwtOptions
{
    public const string Section = "Jwt";

    [Required]
    public string Issuer { get; init; } = "eim-api";

    [Required]
    public string Audience { get; init; } = "eim-clients";

    [Required]
    public string RefreshAudience { get; init; } = "eim-refresh";

    [Required, MinLength(32)]
    public string SigningKey { get; init; } = "";

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 15;

    [Range(1, 90)]
    public int RefreshTokenDays { get; init; } = 7;
}

public sealed class SeedOptions
{
    public const string Section = "Seed";

    [Required, EmailAddress]
    public string RootEmail { get; init; } = "root@eim.local";

    [Required, MinLength(8)]
    public string RootPassword { get; init; } = "";

    [Required]
    public string RootFullName { get; init; } = "System Root";
}

public sealed class DemoOptions
{
    public const string Section = "Demo";

    public bool Enabled { get; init; }
    public bool SeedOnStartup { get; init; }

    [MinLength(8)]
    public string Password { get; init; } = "Demo@12345";
}

public sealed class JobOptions
{
    public const string Section = "Jobs";

    [Range(1, 1440)]
    public int OverdueIntervalMinutes { get; init; } = 60;
}

public sealed class ClockOptions
{
    public const string Section = "Clock";

    public string TimeZone { get; init; } = "Asia/Ho_Chi_Minh";
}
