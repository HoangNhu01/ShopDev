using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class ChangePasswordDto
    {
        [CustomMaxLength(128)]
        public string? OldPassword { get; set; }

        [CustomMaxLength(128)]
        [CustomRequired(AllowEmptyStrings = false)]
        public string? NewPassword { get; set; }
    }
}
