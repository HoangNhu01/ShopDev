using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShopDev.ApplicationBase.Common.Validations
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        public string[]? AllowableExtentions { get; set; }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not IFormFile file)
            {
                throw new InvalidCastException("Field is not instance of IFormFile");
            }

            AllowableExtentions ??= validationContext.GetFileExtensions();

            if (AllowableExtentions?.Contains(Path.GetExtension(file.FileName).ToLower()) == true)
            {
                return ValidationResult.Success;
            }

            ErrorMessage = validationContext.Localize("error_validation_FileExtention");
            var msg = string.Format(ErrorMessage, string.Join(", ", AllowableExtentions ?? []));
            return new ValidationResult(msg);
        }
    }
}
