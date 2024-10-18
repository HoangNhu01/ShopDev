using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class AppChangePasswordDto
    {
        /// <summary>
        /// Mật khẩu cũ
        /// </summary>
        [CustomMaxLength(128)]
        public required string OldPassword { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [CustomMaxLength(128)]
        public required string NewPassword { get; set; }
    }
}
