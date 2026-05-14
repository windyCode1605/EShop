using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Orders
{
    [Table(nameof(OrderItem), Schema = DbSchemas.Default)]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }

        [Required, MaxLength(256)]
        public string ProductName { get; set; } = null!;

        [Required, MaxLength(100), Unicode(false)]
        public string VariantSKU { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
        public virtual ICollection<OrderRefund> Refunds { get; set; } = new List<OrderRefund>();
    }
}
