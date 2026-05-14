using System.ComponentModel.DataAnnotations;
using CR.ApplicationBase.Localization;

namespace CR.DtoBase.Validations
{
    public class CustomMaxLengthAttribute : MaxLengthAttribute, IValidationAttribute
    {
        public CustomMaxLengthAttribute(int length)
            : base(length) { }

        public string? ErrorMessageLocalization { get; set; }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var localization = validationContext.GetService(typeof(LocalizationBase)) as LocalizationBase;
            var errorMessageKey = ErrorMessageLocalization ?? "error_validation_field_MaxLength";
            var localizedMessage = localization?.Localize(errorMessageKey) ?? errorMessageKey;
            
            ErrorMessage = string.Format(localizedMessage, Length);
            return base.IsValid(value, validationContext);
        }
    }
}
