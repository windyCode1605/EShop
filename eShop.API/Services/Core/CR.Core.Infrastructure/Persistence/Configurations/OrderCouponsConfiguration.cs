using CR.Core.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class OrderCouponsConfiguration : IEntityTypeConfiguration<OrderCoupons>
    {
        public void Configure(EntityTypeBuilder<OrderCoupons> entity)
        {
            // Composite primary key: an order can only use each coupon once
            entity.HasKey(oc => new { oc.OrderId, oc.CouponId });

            // Foreign key to Orders
            entity.HasOne(oc => oc.Order)
                .WithMany(o => o.OrderCoupons)
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Coupons
            entity.HasOne(oc => oc.Coupon)
                .WithMany(c => c.OrderCoupons)
                .HasForeignKey(oc => oc.CouponId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}