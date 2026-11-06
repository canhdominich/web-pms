using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Infrastructure.Persistence;

internal static class MySqlSetup
{
    public static DbContextOptionsBuilder UseTLighthouseMySql(this DbContextOptionsBuilder builder, string connectionString, string serverVersion) =>
        builder
            .UseMySql(connectionString, new MySqlServerVersion(Version.Parse(serverVersion)),
                mysql => mysql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention();
}
