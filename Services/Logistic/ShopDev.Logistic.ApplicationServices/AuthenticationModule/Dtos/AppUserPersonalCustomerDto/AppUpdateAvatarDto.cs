using Microsoft.AspNetCore.Http;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    public class AppUpdateAvatarDto
    {
        /// <summary>
        /// File ảnh đại diện
        /// </summary>
        public IFormFile? AvatarImage { get; set; }
    }
}
