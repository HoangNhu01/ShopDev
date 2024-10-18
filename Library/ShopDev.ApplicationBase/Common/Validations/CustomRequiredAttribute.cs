using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
{
    public class CustomRequiredAttribute : RequiredAttribute, IValidationAttribute
    {
        /// <summary>
        /// Key cần dịch
        /// </summary>
        public string? ErrorMessageLocalization { get; set; }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            ErrorMessage = validationContext.Localize(
                ErrorMessageLocalization ?? "error_validation_field_Required"
            );
            return base.IsValid(value, validationContext);
        }
    }
}
