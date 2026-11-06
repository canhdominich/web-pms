using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIM.Infrastructure.Persistence.Configurations;

internal sealed class FeePlanConfiguration : IEntityTypeConfiguration<FeePlan>
{
    public void Configure(EntityTypeBuilder<FeePlan> builder)
    {
        builder.ToTable("finance_fee_plans", t => t.HasCheckConstraint("ck_finance_fee_plans_amount", "amount > 0"));
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.Property(f => f.Currency).HasDefaultValueSql("('VND')").IsRequired();
        builder.Property(f => f.SessionsPerWeek).HasDefaultValue(2);
    }
}

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("finance_invoices", t =>
        {
            t.HasCheckConstraint("ck_finance_invoices_amount", "amount > 0");
            t.HasCheckConstraint("ck_finance_invoices_status", Sql.In("status", InvoiceStatuses.All));
        });
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Status).IsRequired();
        builder.HasOne(i => i.Enrollment).WithMany(e => e.Invoices).HasForeignKey(i => i.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(i => new { i.Status, i.DueDate }).HasPrefixLength(Sql.StatusPrefix, 0);
        builder.HasIndex(i => i.DueDate);
    }
}

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("finance_payments", t =>
        {
            t.HasCheckConstraint("ck_finance_payments_amount", "amount > 0");
            t.HasCheckConstraint("ck_finance_payments_method", Sql.In("method", PaymentMethods.All));
        });
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Method).IsRequired();
        builder.HasOne(p => p.Invoice).WithMany(i => i.Payments).HasForeignKey(p => p.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => p.PaidAt);
    }
}
