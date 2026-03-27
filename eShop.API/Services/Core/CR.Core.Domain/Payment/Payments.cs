using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Payment;
using CR.Core.Domain.Order;

namespace CR.Core.Domain.Payment
{
    [Table(nameof(Payments), Schema = DbSchemas.CRCore)]
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

        [ForeignKey(nameof(OrderId))]
        public Orders Order { get; set; } = null!;
    }
}