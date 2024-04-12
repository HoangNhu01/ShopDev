using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AppUserPersonalCustomerDto
{
    public class AppChangePasswordFirstTimeDto
    {
        /// <summary>
        /// Mật khẩu thay đổi
        /// </summary>
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Password { get; set; }
    }
}
