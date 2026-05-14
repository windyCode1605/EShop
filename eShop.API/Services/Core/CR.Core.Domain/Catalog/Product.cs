using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Catalog
{
    [Table(nameof(Product), Schema = DbSchemas.Default)]
    [Index(nameof(Slug), IsUnique = true, Name = $"IX_{nameof(Product)}_Slug")]
    public class Product : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(256), Unicode(false)]
        public string Slug { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public string? Description { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<Review.Reviews> Reviews { get; set; } = new List<Review.Reviews>();
    }
}
