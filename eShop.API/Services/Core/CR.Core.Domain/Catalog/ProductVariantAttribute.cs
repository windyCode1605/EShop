using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng trung gian ProductVariantAttribute — nối ProductVariant với Attribute và AttributeValue.
    /// Cho phép mỗi variant có nhiều thuộc tính động (Color: Red, Size: XL, ...).
    /// </summary>
    [Table(nameof(ProductVariantAttribute), Schema = DbSchemas.Default)]
    public class ProductVariantAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductVariantId { get; set; }

        public int AttributeId { get; set; }

        /// <summary>Nullable — có thể null khi dùng CustomValue thay thế.</summary>
        public int? AttributeValueId { get; set; }

        /// <summary>Giá trị tùy chỉnh khi không dùng AttributeValue định nghĩa sẵn.</summary>
        [MaxLength(255)]
        public string? CustomValue { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        // Navigation properties
        public virtual ProductVariant ProductVariant { get; set; } = null!;
        public virtual Attribute Attribute { get; set; } = null!;
        public virtual AttributeValue? AttributeValue { get; set; }
    }
}
