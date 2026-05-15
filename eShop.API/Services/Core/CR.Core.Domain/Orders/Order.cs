using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Logistics;
using CR.Core.Domain.Payment;
using CR.Core.Domain.User;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Orders
{
    [Table(nameof(Order), Schema = DbSchemas.Default)]
    [Index(nameof(OrderCode), IsUnique = true, Name = $"IX_{nameof(Order)}_OrderCode")]
    public class Order : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string OrderCode { get; set; } = null!;

        public int UserId { get; set; }

        [Required, MaxLength(1024)]
        public string ShippingAddress { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string Status { get; set; } = null!;

        [Required, MaxLength(50), Unicode(false)]
        public string PaymentMethod { get; set; } = null!;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public virtual Users User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
