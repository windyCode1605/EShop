using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Catalog;
using CR.EntitiesBase.Entities;

namespace CR.Core.Domain.Carts
{
    [Table(nameof(CartItem), Schema = DbSchemas.Default)]
    public class CartItem : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CartId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }

        public virtual Cart Cart { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
