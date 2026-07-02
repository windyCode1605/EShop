using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Payment;
using CR.Core.Domain.Orders;

namespace CR.Core.Domain.Payment
{
    [Table(nameof(Payments), Schema = DbSchemas.Default)]
    public class Payments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        public string Method { get; set; } = PaymentMethod.Cash.ToString();

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = PaymentStatus.Pending.ToString();

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        /// <summary>
        /// PaitAt la thời điểm thanh toán thành công, có thể null nếu chưa thanh toán hoặc thanh toán thất bại
        /// </summary>

        public DateTime? PaidAt { get; set; } 

        [MaxLength(200)]
        public string? TransactionId { get; set; }

        [MaxLength(20)]
        public string? GatewayResponseCode { get; set; }

        public string? GatewayResponseRaw { get; set; }

        public string? PaymentUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundedAmount { get; set; }

        public DateTime? RefundedAt { get; set; }

        [MaxLength(500)]
        public string? RefundReason { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;
    }
}
