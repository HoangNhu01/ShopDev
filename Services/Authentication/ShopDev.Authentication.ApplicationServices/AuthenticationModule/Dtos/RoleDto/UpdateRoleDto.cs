namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto
{
    public class UpdateRoleDto
    {
        public int UserId { get; set; }
        public List<int>? RoleIds { get; set; }
    }
}
