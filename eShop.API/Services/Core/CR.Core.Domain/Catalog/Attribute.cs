using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng Attribute — lưu danh sách thuộc tính của sản phẩm (ví dụ: Color, Size, Material...).
    /// Lưu ý: CreatedBy, ModifiedBy, DeletedBy trong DB là nvarchar(100) nên không kế thừa AuditableEntity.
    /// </summary>
    [Table("Attribute", Schema = DbSchemas.Default)]
    public class Attribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>Text, Number, Color, Boolean...</summary>
        [Required, MaxLength(20)]
        public string AttributeType { get; set; } = null!;

        public bool IsFilterable { get; set; }

        public int DisplayOrder { get; set; }

        // Audit fields — kiểu string vì DB định nghĩa nvarchar(100)
        public DateTime CreatedDate { get; set; }
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        [MaxLength(100)]
        public string? DeletedBy { get; set; }
        public bool Deleted { get; set; }

        // Navigation properties
        public virtual ICollection<AttributeValue> Values { get; set; } = new List<AttributeValue>();
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new List<ProductVariantAttribute>();
    }
}
