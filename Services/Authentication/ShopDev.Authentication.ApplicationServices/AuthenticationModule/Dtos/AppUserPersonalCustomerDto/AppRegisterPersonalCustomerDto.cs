using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    public class AppRegisterPersonalCustomerDto
    {
        private string _phone = null!;

        [Phone]
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Phone
        {
            get => _phone;
            set => _phone = value.Trim();
        }

        private string _email = null!;

        [Email]
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Email
        {
            get => _email;
            set => _email = value.Trim();
        }

        private string? _referralCode;

        /// <summary>
        /// Mã giới thiệu tạo tài khoản
        /// </summary>
        public string? ReferralCode
        {
            get => _referralCode;
            set => _referralCode = value?.Trim();
        }
    }
}
