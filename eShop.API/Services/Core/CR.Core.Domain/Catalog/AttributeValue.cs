using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng AttributeValue — lưu các giá trị của từng thuộc tính (ví dụ: Red, Blue, XL, XXL...).
    /// </summary>
    [Table(nameof(AttributeValue), Schema = DbSchemas.Default)]
    public class AttributeValue : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AttributeId { get; set; }

        [Required, MaxLength(100)]
        public string Value { get; set; } = null!;

        [MaxLength(7)]
        public string? ColorHex { get; set; }

        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Attribute Attribute { get; set; } = null!;
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new List<ProductVariantAttribute>();
    }
}
