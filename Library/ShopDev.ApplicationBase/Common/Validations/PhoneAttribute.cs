using System.ComponentModel.DataAnnotations;

namespace ShopDev.ApplicationBase.Common.Validations
{
    /// <summary>
    /// Kiểm tra số điện thoại
    /// </summary>
    public class PhoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Danh sách đầu số các nhà mạng
        /// </summary>
        private readonly string[] prefixList =
        {
            "086",
            "096",
            "097",
            "098",
            "032",
            "033",
            "034",
            "035",
            "036",
            "037",
            "038",
            "039",
            "088",
            "091",
            "094",
            "083",
            "084",
            "085",
            "081",
            "082",
            "089",
            "090",
            "093",
            "070",
            "079",
            "077",
            "076",
            "078",
            "092",
            "056",
            "058",
            "087",
            "099",
            "059",
            // đầu số hỗ trợ
            "054",
            // để dùng cho trường hợp đặc biệt
            "000"
        };

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null)
                return ValidationResult.Success;

            string phone = value.ToString()!;

            if (string.IsNullOrEmpty(phone))
            {
                return ValidationResult.Success;
            }

            ErrorMessage = validationContext.Localize("error_validation_PhoneInvalid");
            if (phone.Length != 10)
            {
                return new ValidationResult(ErrorMessage);
            }

            bool prefixValid = false;

            for (int i = 0; i < prefixList.Length; i++)
            {
                var item = prefixList[i];
                if (phone.StartsWith(item))
                {
                    prefixValid = true;
                    break;
                }
            }

            if (!prefixValid)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
