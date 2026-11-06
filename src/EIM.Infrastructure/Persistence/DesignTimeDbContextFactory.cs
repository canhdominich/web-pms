using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EIM.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
                               ?? "Server=localhost;Port=3306;Database=eim;User=eim;Password=eim";
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseEimMySql(connectionString, "8.0.36");
        return new AppDbContext(builder.Options);
    }
}
