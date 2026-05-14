using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Catalog
{
    [Table(nameof(ProductVariant), Schema = DbSchemas.Default)]
    [Index(nameof(SKU), IsUnique = true, Name = $"IX_{nameof(ProductVariant)}_SKU")]
    public class ProductVariant : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string SKU { get; set; } = null!;

        [MaxLength(50)]
        public string? Size { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAdjustment { get; set; }

        public int StockQuantity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
