using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Coupons;

namespace CR.Core.Domain.Order
{
    [Table(nameof(OrderCoupons), Schema = DbSchemas.CRCore)]
    public class OrderCoupons
    {
        public int OrderId { get; set; }

        public int CouponId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountApplied { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Orders Order { get; set; } = null!;

        [ForeignKey(nameof(CouponId))]
        public Coupons.Coupons Coupon { get; set; } = null!;
    }
}