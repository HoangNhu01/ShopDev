namespace ShopDev.Authentication.API.Models
{
    public class AuthenticateModel
    {
        /// <summary>
        /// Tài khoản
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Đường dẫn điều hướng lại connect/authorize
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}
