using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TLighthouse.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
                               ?? "Server=localhost;Port=3306;Database=tlighthouse;User=tlighthouse;Password=tlighthouse";
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseTLighthouseMySql(connectionString, "8.0.36");
        return new AppDbContext(builder.Options);
    }
}
