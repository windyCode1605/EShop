using CR.Core.Domain.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CouponsConfiguration : IEntityTypeConfiguration<Coupons>
    {
        public void Configure(EntityTypeBuilder<Coupons> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property( c => c.Code).IsRequired().HasMaxLength(50);
            entity.Property(c => c.DiscountType).IsRequired();
            entity.Property(c => c.DiscountValue).IsRequired();
            entity.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
            entity.Property(c => c.ExpiryDate).IsRequired();
            entity.Property(c => c.IsActive).IsRequired();
            entity.Property(c => c.MinOrderValue).HasColumnType("decimal(18,2)");
            entity.Property(c => c.MaxDiscountValue).HasColumnType("decimal(18,2)");

            entity.HasMany(c => c.OrderCoupons)
            .WithOne(oc =>  oc.Coupon)
            .HasForeignKey(oc => oc.CouponId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}