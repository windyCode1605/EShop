using System.ComponentModel.DataAnnotations;

namespace CR.DtoBase.Validations
{
    public class CustomRequiredAttribute : RequiredAttribute, IValidationAttribute
    {
        /// <summary>
        /// ErrorMessageLocalization cho phép bạn cung cấp một khóa để tra cứu thông điệp lỗi đã được dịch trong các tài nguyên ngôn ngữ khác nhau. Khi thuộc tính này được thiết lập, hệ thống sẽ sử dụng khóa này để tìm kiếm thông điệp lỗi tương ứng trong tài nguyên ngôn ngữ thay vì sử dụng thông điệp lỗi mặc định hoặc thông điệp lỗi được cung cấp trực tiếp qua ErrorMessage. Điều này rất hữu ích trong các ứng dụng đa ngôn ngữ, giúp đảm bảo rằng người dùng nhận được thông điệp lỗi phù hợp với ngôn ngữ của họ.
        /// </summary>
        public string? ErrorMessageLocalization { get; set; } 

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           ErrorMessage = ErrorMessageLocalization ?? $"error_validation_field_required";
            return base.IsValid(value, validationContext);
        }
    }
}