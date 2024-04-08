using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    /// <summary>
    /// Thay đổi mật khẩu
    /// </summary>
    public class AppChangePasswordForgotDto
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
        /// Mật khẩu thay đổi
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Password { get; set; }
    }
}
