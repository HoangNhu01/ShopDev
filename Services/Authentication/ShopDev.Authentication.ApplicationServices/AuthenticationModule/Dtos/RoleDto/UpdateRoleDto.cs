using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto
{
    public class UpdateRoleDto
    {
        public int UserId { get; set; }
        public List<CreateUserRoleDto> UserRoles { get; set; } = [];
    }
}
