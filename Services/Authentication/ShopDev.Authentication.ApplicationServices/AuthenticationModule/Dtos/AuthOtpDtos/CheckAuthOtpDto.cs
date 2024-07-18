using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AuthOtpDto
{
    public class CheckAuthOtpDto
    {
        private string _phone = null!;

        [CustomRequired(AllowEmptyStrings = false)]
        public required string Phone
        {
            get => _phone;
            set => _phone = value.Trim();
        }

        private string _otpCode = null!;

        [CustomRequired(AllowEmptyStrings = false)]
        [CustomMaxLength(6)]
        public required string OtpCode
        {
            get => _otpCode;
            set => _otpCode = value.Trim();
        }
    }
}
