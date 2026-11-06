using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TLighthouse.Infrastructure.Persistence.Configurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("auth_users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.FullName).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.Status).HasColumnType("varchar(50)").HasDefaultValue(UserStatuses.Active).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}

internal sealed class AuthRoleConfiguration : IEntityTypeConfiguration<AuthRole>
{
    public void Configure(EntityTypeBuilder<AuthRole> builder)
    {
        builder.ToTable("auth_roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Code).HasColumnType("varchar(100)").IsRequired();
        builder.Property(r => r.Name).HasColumnType(Sql.Varchar255).IsRequired();
        builder.HasIndex(r => r.Code).IsUnique();
    }
}
