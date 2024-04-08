using System.ComponentModel.DataAnnotations;
using ShopDev.ApplicationBase.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.ApplicationBase.Common.Validations
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Dịch error message localization
        /// </summary>
        /// <returns></returns>
        public static string Localize(
            this ValidationContext validationContext,
            string errorMessageLocalization
        )
        {
            var localization = validationContext.GetRequiredService<LocalizationBase>();
            return localization.Localize(errorMessageLocalization);
        }

        /// <summary>
        /// Lấy maxlength file trong appsettings.json
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static long GetFileMaxLength(this ValidationContext validationContext)
        {
            var config = validationContext.GetRequiredService<IConfiguration>();
            string limitLengthStr =
                config["FileConfig:File:LimitUpload"]?.ToString()
                ?? throw new InvalidOperationException(
                    "have not configured key=\"FileConfig:File:LimitUpload\" in appsetting"
                );
            return long.Parse(limitLengthStr);
        }

        public static string[] GetFileExtensions(this ValidationContext validationContext)
        {
            var config = validationContext.GetRequiredService<IConfiguration>();
            string[] extenstions =
                config["FileConfig:File:AllowExtension"]?.Split(",")
                ?? throw new InvalidOperationException(
                    "have not configured key=\"FileConfig:File:AllowExtension\" in appsetting"
                );
            return extenstions;
        }
    }
}
