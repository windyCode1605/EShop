using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Orders
{
    [Table(nameof(OrderRefund), Schema = DbSchemas.Default)]
    public class OrderRefund : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderItemId { get; set; }
        public int RefundQuantity { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string Status { get; set; } = "PENDING";

        public virtual OrderItem OrderItem { get; set; } = null!;
    }
}
