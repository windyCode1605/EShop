using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Discount;
using CR.Core.Domain.Order;
using CR.EntitiesBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Coupons
{


    [Table(nameof(Coupons), Schema = DbSchemas.CRCore)]
    [Index(nameof(Code), IsUnique = true)]
    public class Coupons : BaseEntity
    {
        /// <summary>
        /// Mã coupon (duy nhất), user nhập để áp dụng giảm giá
        /// Ví dụ: SALE50, FREESHIP, NEWUSER
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Loại giảm giá:
        /// - Percentage: giảm theo %
        /// - Fixed: giảm số tiền cố định
        /// </summary>
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// Giá trị giảm:
        /// - Nếu Percentage: là % (vd: 10 = giảm 10%)
        /// - Nếu Fixed: là số tiền (vd: 50000 = giảm 50k)
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Giá trị đơn hàng tối thiểu để áp dụng coupon
        /// Nếu null: không yêu cầu
        /// </summary>
        public decimal? MinOrderValue { get; set; }

        /// <summary>
        /// Số tiền giảm tối đa (chỉ áp dụng khi DiscountType = Percentage)
        /// Tránh trường hợp giảm quá nhiều (vd đơn 10tr giảm 50%)
        /// </summary>
        public decimal? MaxDiscountValue { get; set; }

        /// <summary>
        /// Thời điểm bắt đầu có hiệu lực của coupon
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Thời điểm hết hạn coupon
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Tổng số lần coupon được phép sử dụng (global)
        /// Nếu null: không giới hạn
        /// </summary>
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Số lần coupon đã được sử dụng
        /// Dùng để check với UsageLimit
        /// </summary>
        public int UsedCount { get; set; } = 0;

        /// <summary>
        /// Số lần tối đa mỗi user được sử dụng coupon này
        /// Nếu null: không giới hạn theo user
        /// </summary>
        public int? UsageLimitPerUser { get; set; }

        /// <summary>
        /// Trạng thái hoạt động của coupon
        /// true = đang hoạt động
        /// false = đã tắt (admin disable)
        /// </summary>
        public bool IsActive { get; set; } = true;
        public ICollection<OrderCoupons> OrderCoupons { get; set; } = new List<OrderCoupons>();
    }
}