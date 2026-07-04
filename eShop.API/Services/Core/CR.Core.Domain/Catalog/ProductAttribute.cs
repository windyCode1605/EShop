using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng trung gian ProductAttribute — nối Product với Attribute và AttributeValue.
    /// Dành cho các thuộc tính mô tả ở cấp độ Product (Material, Brand, Origin...), không tạo ra Variant.
    /// </summary>
    [Table(nameof(ProductAttribute), Schema = DbSchemas.Default)]
    public class ProductAttribute : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int AttributeId { get; set; }

        /// <summary>Nullable — có thể null khi dùng CustomValue thay thế.</summary>
        public int? AttributeValueId { get; set; }

        /// <summary>Giá trị tùy chỉnh khi không dùng AttributeValue định nghĩa sẵn.</summary>
        [MaxLength(255)]
        public string? CustomValue { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual Attribute Attribute { get; set; } = null!;
        public virtual AttributeValue? AttributeValue { get; set; }
    }
}
