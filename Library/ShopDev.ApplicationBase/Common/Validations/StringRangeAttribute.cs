using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[]? AllowableValues { get; set; }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null || AllowableValues?.Contains(value.ToString()) == true)
            {
                return ValidationResult.Success;
            }
            ErrorMessage ??= validationContext.Localize("error_validation_StringRange");
            var msg = string.Format(ErrorMessage, string.Join(", ", AllowableValues ?? []));
            return new ValidationResult(msg);
        }
    }
}
