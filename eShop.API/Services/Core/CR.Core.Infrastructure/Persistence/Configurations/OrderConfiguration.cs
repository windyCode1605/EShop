using CR.Core.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Orders>
    {

        public void Configure(EntityTypeBuilder<Orders> entity)
        {
             entity.HasKey(o => o.Id);
            entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            entity.Property(o => o.OrderStatus).IsRequired();
            entity.Property(o => o.PlacedAt).IsRequired();

            // Relationships
            entity.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.OrderCoupons)
                .WithOne(oc => oc.Order)
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}