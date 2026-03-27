using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase;

namespace CR.Core.Domain.Product
{
    [Table(nameof(Products), Schema = DbSchemas.CRCore)]
    public class Products : BaseEntity
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; } // Số lượng tồn kho

        /// <summary>
        /// Slug là một chuỗi duy nhất được sử dụng để định danh sản phẩm trong URL. 
        /// Nó thường được tạo ra từ tên sản phẩm bằng cách chuyển đổi thành chữ thường, 
        /// thay thế khoảng trắng bằng dấu gạch ngang và loại bỏ các ký tự đặc biệt. 
        /// Slug giúp cải thiện SEO và làm cho URL dễ đọc hơn.
        /// </summary>
        [MaxLength(200)]
        public string Slug { get; set; } = null!;

        public string? Description { get; set; }
        public string? AI_Description { get; set; }
        public bool AI_Generated { get; set; }

        public bool IsActive { get; set; } = true;
        [ForeignKey(nameof(CategoryId))]
        public Category.Categories Category { get; set; } = null!;

        public ICollection<ProductVariants> Variants { get; set; } = new List<ProductVariants>();
        public ICollection<ProductImages> Images { get; set; } = new List<ProductImages>();
        public ICollection<Review.Reviews> Reviews { get; set; } = new List<Review.Reviews>();
    }
}