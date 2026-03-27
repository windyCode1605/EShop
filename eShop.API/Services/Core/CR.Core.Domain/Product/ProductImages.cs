using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Product
{
    [Table(nameof(ProductImages), Schema = DbSchemas.CRCore)]
    public class ProductImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        public int SortOrder { get; set; } = 0;

        public bool IsPrimary { get; set; } = false;

        [ForeignKey(nameof(ProductId))]
        public Products? Product { get; set; } = null!;
    }
}