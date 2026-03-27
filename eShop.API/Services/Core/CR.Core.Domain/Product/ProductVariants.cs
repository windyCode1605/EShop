using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Cart;
using CR.EntitiesBase;

namespace CR.Core.Domain.Product
{
    [Table(nameof(ProductVariants), Schema = DbSchemas.CRCore)]
    public class ProductVariants : BaseEntity
    {
    

        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Sku { get; set; } = null!;

        [MaxLength(50)]
        public string? Color { get; set; }

        [MaxLength(20)]
        public string? Size { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQty { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Products Product { get; set; } = null!;

        public ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }

    
}