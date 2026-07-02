using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng AttributeValue — lưu các giá trị của từng thuộc tính (ví dụ: Red, Blue, XL, XXL...).
    /// </summary>
    [Table(nameof(AttributeValue), Schema = DbSchemas.Default)]
    public class AttributeValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AttributeId { get; set; }

        [Required, MaxLength(100)]
        public string Value { get; set; } = null!;

        public int DisplayOrder { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        // Navigation properties
        public virtual Attribute Attribute { get; set; } = null!;
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new List<ProductVariantAttribute>();
    }
}
