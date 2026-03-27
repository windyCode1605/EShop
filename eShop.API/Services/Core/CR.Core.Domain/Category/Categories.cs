using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Category
{
    [Table(nameof(Categories), Schema = DbSchemas.CRCore)]
    public class Categories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = null!;

        // [ForeignKey(nameof(ParentId))]
        // public Categories? Parent { get; set; }

        public ICollection<Categories> Children { get; set; } = new List<Categories>();
        public ICollection<Product.Products> Products { get; set; } = new List<Product.Products>();
    }
}