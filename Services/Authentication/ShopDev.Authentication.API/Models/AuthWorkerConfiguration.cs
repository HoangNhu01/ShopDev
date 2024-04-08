namespace ShopDev.Authentication.API.Models
{
    public class AuthWorkerConfiguration
    {
        /// <summary>
        /// Đường dẫn điều hướng đăng nhập
        /// </summary>
        public List<string> RedirectUris { get; set; } = new List<string>();

        /// <summary>
        /// Đường dẫn chuyển hướng đăng xuất
        /// </summary>
        public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
    }
}
