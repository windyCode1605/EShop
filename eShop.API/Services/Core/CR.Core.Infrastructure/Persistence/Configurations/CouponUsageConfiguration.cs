using CR.Core.Domain.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> entity)
        {
            entity.HasKey(cu => cu.Id);

            entity.Property(cu => cu.CouponId).IsRequired();
            entity.Property(cu => cu.UserId).IsRequired();
            entity.Property(cu => cu.OrderId).IsRequired();
            entity.Property(cu => cu.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(cu => cu.UsedAt).IsRequired();

            // Index để hỗ trợ truy vấn nhanh: "Người dùng này đã xài mã này bao nhiêu lần?"
            entity.HasIndex(cu => new { cu.UserId, cu.CouponId })
                .HasName($"IX_{nameof(CouponUsage)}_UserId_CouponId");

            // FK → Coupons (Restrict: không cho xóa Coupon nếu đã có người xài)
            entity.HasOne(cu => cu.Coupon)
                .WithMany()
                .HasForeignKey(cu => cu.CouponId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK → Users (Restrict: không cho xóa User nếu có lịch sử xài coupon - tránh cascade cycle)
            entity.HasOne(cu => cu.User)
                .WithMany()
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK → Order (Cascade: khi hủy đơn hàng thì xóa luôn vết xài Coupon để user xài lại)
            entity.HasOne(cu => cu.Order)
                .WithMany()
                .HasForeignKey(cu => cu.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(cu => !cu.Order.Deleted);
        }
    }
}
