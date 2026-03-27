

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Cart
{
[Table(nameof(CartItems), Schema = DbSchemas.CRCore)]
    public class CartItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CartId { get; set; }

        public int VariantId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(CartId))]
        public Carts Cart { get; set; } = null!;

        [ForeignKey(nameof(VariantId))]
        public Product.ProductVariants Variant { get; set; } = null!;
    }
}