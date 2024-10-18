using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
{
    /// <summary>
    /// Kiểm tra địa chỉ email
    /// </summary>
    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is null)
                return ValidationResult.Success;

            string email = value.ToString()!;
            string trimmedEmail = email.Trim();

            if (string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success;
            }

            ErrorMessage = validationContext.Localize("error_validation_EmailInvalid");
            if (trimmedEmail.EndsWith('.'))
            {
                return new ValidationResult(ErrorMessage);
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address == trimmedEmail)
                {
                    return ValidationResult.Success;
                }
            }
            catch
            {
                return new ValidationResult(ErrorMessage);
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
