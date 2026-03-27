using CR.Core.Domain.Payment;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payments>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Payments> entity)
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Method).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}