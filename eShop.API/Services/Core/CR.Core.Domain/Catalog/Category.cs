using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Catalog
{
    [Table(nameof(Category), Schema = DbSchemas.Default)]
    [Index(nameof(Slug), IsUnique = true, Name = $"IX_{nameof(Category)}_Slug")]
    public class Category : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(256), Unicode(false)]
        public string Slug { get; set; } = null!;

        public virtual Category? Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();
    }
}
