using System.ComponentModel.DataAnnotations;

namespace CR.DtoBase.Validations
{
    /// <summary>
    /// Kiểm tra đinh dạng Email
    /// </summary>
    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null )
            {
                return ValidationResult.Success; // Không kiểm tra nếu giá trị là null hoặc chuỗi rỗng
            }
            string email = value.ToString()!;
            string trimmedEmail = email.Trim();
            if(string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success; // Không kiểm tra nếu giá trị là null hoặc chuỗi rỗng
            }
            var errorMessage = ErrorMessage ?? $"{validationContext.DisplayName} không đúng định dạng email.";
            if(trimmedEmail.EndsWith("."))
            {
                return new ValidationResult(errorMessage);
            }
            try 
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if(addr.Address == trimmedEmail)
                {
                    return  ValidationResult.Success;
                }
            }
            catch 
            {
                return new ValidationResult(errorMessage);
            }
            return new ValidationResult(errorMessage); 
        }
    }
}