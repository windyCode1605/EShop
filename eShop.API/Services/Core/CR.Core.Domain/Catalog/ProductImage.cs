using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;

namespace CR.Core.Domain.Catalog
{
    [Table(nameof(ProductImage), Schema = DbSchemas.Default)]
    public class ProductImage : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
