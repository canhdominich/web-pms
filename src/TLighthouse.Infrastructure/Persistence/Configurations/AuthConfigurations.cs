using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TLighthouse.Infrastructure.Persistence.Configurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.FullName).HasColumnType(Sql.Varchar255).IsRequired();
        builder.Property(u => u.Role).HasColumnType("varchar(50)").IsRequired();
        builder.Property(u => u.Status).HasColumnType("varchar(50)").HasDefaultValue(UserStatuses.Active).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.ToTable("auth_users", t => t.HasCheckConstraint("ck_auth_users_role", Sql.In("role", RoleCodes.Names.Keys)));
    }
}
