using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    /// <summary>
    /// Xác thực mã Otp khi quên mật khẩu
    /// </summary>
    public class AppConfirmOtpPasswordForgotDto
    {
        /// <summary>
        /// Số điện thoại người dùng
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Phone { get; set; }

        /// <summary>
        /// Mã xác nhận quên mật khẩu
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public required string SecretPasswordCode { get; set; }

        /// <summary>
        /// Mã Otp
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public required string OtpCode { get; set; }
    }
}
