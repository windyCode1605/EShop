using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Shipment;
using CR.Core.Domain.Orders;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Logistics
{
    /// <summary>
    /// Bảng Vận đơn (Shipment) - ghi vết thông tin vận chuyển từng kiện hàng
    /// - Hỗ trợ tính năng Tách kiện hàng (Split Shipments) khi đơn chứa nhiều sản phẩm ở các kho khác nhau
    /// - Tách riêng khỏi Order để Order chỉ đóng vai trò "chứng từ thương mại" (lưu thanh toán + tổng tiền)
    /// - Cho phép một đơn hàng có 1 hoặc nhiều Shipment (VD: 1 order → 2 shipments nếu hàng từ 2 kho khác nhau)
    /// </summary>
    [Table(nameof(Shipment), Schema = DbSchemas.Default)]
    [Index(nameof(TrackingNumber), IsUnique = false, Name = $"IX_{nameof(Shipment)}_TrackingNumber")]
    public class Shipment : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Đơn hàng tương ứng (1 Order → 1 hoặc nhiều Shipment)
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Tên đơn vị vận chuyển (VD: "GHN", "NinjaVan", "GrabExpress", "Viettel Post", "GHTK")
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ShippingProvider { get; set; } = null!;

        /// <summary>
        /// Mã vận đơn (Tracking Number) do đối tác vận chuyển cấp
        /// Có thể null nếu chưa đưa cho đơn vị vận chuyển
        /// </summary>
        [MaxLength(100)]
        [Unicode(false)]
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Phí vận chuyển thực tế
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// Tên người nhận (Khách có thể đặt mua hộ người khác)
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string ReceiverName { get; set; } = null!;

        /// <summary>
        /// Số điện thoại người nhận
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = null!;

        /// <summary>
        /// Địa chỉ giao hàng (Có thể khác với địa chỉ trong Order nếu đơn chứa nhiều người nhận)
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string ShippingAddress { get; set; } = null!;

        /// <summary>
        /// Ngày dự kiến giao (Thông tin từ đơn vị vận chuyển)
        /// </summary>
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// Ngày giao thành công (Khi trạng thái = DELIVERED)
        /// </summary>
        public DateTime? ActualDelivery { get; set; }

        /// <summary>
        /// Trạng thái vận chuyển:
        /// - PENDING: Chưa gửi cho đơn vị vận chuyển
        /// - PICKED_UP: Đã lấy hàng từ kho
        /// - IN_TRANSIT: Đang trong quá trình vận chuyển
        /// - DELIVERED: Giao thành công
        /// - FAILED: Giao thất bại
        /// - RETURNED: Hàng trả về
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Unicode(false)]
        public string Status { get; set; } = ShipmentStatus.Pending;

        // Navigation Properties
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;
    }
}
