using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TLighthouse.Infrastructure.Persistence.Configurations;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("system_audit_logs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).IsRequired();
        builder.Property(a => a.Entity).IsRequired();
        builder.Property(a => a.Meta).HasColumnType("json").HasDefaultValueSql("(JSON_OBJECT())").IsRequired();
        builder.HasOne(a => a.Actor).WithMany().HasForeignKey(a => a.ActorUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(a => new { a.Entity, a.EntityId }).HasPrefixLength(Sql.CodePrefix, 0);
        builder.HasIndex(a => new { a.ActorUserId, a.Action, a.CreatedAt }).HasPrefixLength(0, Sql.StatusPrefix, 0);
        builder.HasIndex(a => a.CreatedAt);
    }
}

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("system_notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired();
        builder.Property(n => n.Body).IsRequired();
        builder.Property(n => n.IsRead).HasDefaultValue(false);
        builder.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });
    }
}
