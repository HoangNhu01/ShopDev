namespace ShopDev.Authentication.API.Models
{
    public class ConsentModel
    {
        /// <summary>
        /// Đường dẫn điều hướng lại connect/authorize
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Giá trị của Consent
        /// </summary>
        public string? Grant { get; set; }
    }
}
