using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;

namespace CR.Core.Domain.Catalog
{
    /// <summary>
    /// Bảng CategoryAttribute — định nghĩa template các Attribute cho từng Category.
    /// (VD: Áo thun cần Color+Size, Điện thoại cần RAM+ROM).
    /// </summary>
    [Table(nameof(CategoryAttribute), Schema = DbSchemas.Default)]
    public class CategoryAttribute : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int AttributeId { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual Attribute Attribute { get; set; } = null!;
    }
}
