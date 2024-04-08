using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
{
    /// <summary>
    /// Cho phép một trong các giá trị, nếu là null thì bỏ qua
    /// </summary>
    public class IntegerRangeAttribute : ValidationAttribute
    {
        public int[] AllowableValues { get; set; } = null!;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null || AllowableValues?.Contains((int)value) == true)
            {
                return ValidationResult.Success;
            }
            ErrorMessage = validationContext.Localize("error_validation_IntegerRange");
            var msg = string.Format(ErrorMessage, string.Join(", ", AllowableValues ?? []));
            return new ValidationResult(msg);
        }
    }
}
