using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    /// <summary>
    /// Thiết lập mật khẩu khi đăng ký tài khoản
    /// </summary>
    public class AppUserRegisterPasswordDto
    {
        private string _phone = null!;

        /// <summary>
        /// Số điện thoại đăng ký
        /// </summary>
        [Phone]
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Phone
        {
            get => _phone;
            set => _phone = value.Trim();
        }

        /// <summary>
        /// Mật khẩu đăng nhập
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public string Password { get; set; } = null!;
    }
}
