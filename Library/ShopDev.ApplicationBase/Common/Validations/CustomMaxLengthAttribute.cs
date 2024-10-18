using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
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
            ErrorMessage = string.Format(
                validationContext.Localize(
                    ErrorMessageLocalization ?? "error_validation_field_MaxLength"
                ),
                Length
            );
            return base.IsValid(value, validationContext);
        }
    }
}
