using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants;
using CR.Constants.Common.Database;
using CR.Core.Domain.Order;
using CR.EntitiesBase;

namespace CR.Core.Domain.Product
{
    [Table(nameof(OrderItems), Schema = DbSchemas.CRCore)]
    public class OrderItems : BaseEntity
    {
        [Required]
        public int OrderId { get; set; }

        public int VariantId { get; set; } // optional giữ lại để trace

       
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } = null!;

        [MaxLength(255)]
        public string? VariantName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

   
        [ForeignKey(nameof(OrderId))]
        public Orders Order { get; set; } = null!;

        [ForeignKey(nameof(VariantId))]
        public ProductVariants? Variant { get; set; } // optional
    }
}