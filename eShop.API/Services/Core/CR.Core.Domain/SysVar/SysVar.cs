using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.SysVar
{
    [Table(nameof(SysVar), Schema = DbSchemas.Default)]
    [Index(nameof(GrName), nameof(VarName), Name = $"IX_{nameof(SysVar)}")]
    public class SysVar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        /// <summary>
        /// Nhóm biến, dùng để phân loại các biến có liên quan đến nhau, 
        /// ví dụ: nhóm biến cấu hình hệ thống, nhóm biến cấu hình email, nhóm biến cấu hình đăng nhập thất bại,...
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string GrName { get; set; }


        /// <summary>
        /// Tên biến, dùng để định danh biến trong nhóm, ví dụ: LOGINMAXTURN, EMAILHOST, EMAILPORT,...
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string VarName { get; set; }


        /// <summary>
        /// Giá trị của biến, có thể là số, chuỗi, boolean,... tùy thuộc vào loại biến và cách sử dụng của biến đó trong ứng dụng.
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string VarValue { get; set; }

        /// <summary>
        /// Mô tả về biến, giúp người quản trị hiểu được ý nghĩa và cách
        /// sử dụng của biến đó, có thể để trống nếu không cần thiết.
        /// </summary>
        [MaxLength(128)]
        [Unicode(true)]
        public string? VarDesc { get; set; }
    }
}
