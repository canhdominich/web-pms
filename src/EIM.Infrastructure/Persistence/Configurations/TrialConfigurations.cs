using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIM.Infrastructure.Persistence.Configurations;

internal sealed class TrialLeadConfiguration : IEntityTypeConfiguration<TrialLead>
{
    public void Configure(EntityTypeBuilder<TrialLead> builder)
    {
        builder.ToTable("trial_leads", t => t.HasCheckConstraint("ck_trial_leads_status", Sql.In("status", TrialLeadStatuses.All)));
        builder.HasKey(l => l.Id);
        builder.Property(l => l.FullName).IsRequired();
        builder.Property(l => l.Phone).IsRequired();
        builder.Property(l => l.Status).IsRequired();
        builder.HasIndex(l => l.Phone).HasPrefixLength(Sql.PhonePrefix);
        builder.HasIndex(l => l.Status).HasPrefixLength(Sql.StatusPrefix);
        builder.HasOne(l => l.Creator).WithMany().HasForeignKey(l => l.CreatedBy).OnDelete(DeleteBehavior.Restrict);
    }
}
