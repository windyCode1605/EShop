using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Orders;
using CR.Core.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Coupons
{
    /// <summary>
    /// Bảng ghi vết lịch sử sử dụng mã giảm giá
    /// - Hỗ trợ kiểm tra UsageLimitPerUser (Giới hạn số lần dùng mã per user)
    /// - Đóng vai trò audit trail để đối soát kế toán (xem hệ thống đã "đốt" bao nhiêu tiền cho các chiến dịch Marketing)
    /// </summary>
    [Table(nameof(CouponUsage), Schema = DbSchemas.Default)]
    [Index(nameof(UserId), nameof(CouponId), Name = $"IX_{nameof(CouponUsage)}_UserId_CouponId")]
    public class CouponUsage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Mã giảm giá được sử dụng
        /// </summary>
        [Required]
        public int CouponId { get; set; }

        /// <summary>
        /// Người dùng sử dụng mã
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Đơn hàng áp dụng mã (1 đơn chỉ xài 1 mã - hoặc N:1 nếu cho phép chồng lên nhau)
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Số tiền thực tế đã được giảm cho đơn hàng này
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Thời điểm áp dụng mã giảm giá
        /// </summary>
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(CouponId))]
        public virtual Coupons Coupon { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual Users User { get; set; } = null!;

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;
    }
}
