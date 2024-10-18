using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto
{
    public class CreateRolePermissionDto
    {
        private string _name = null!;

        [CustomRequired]
        public required string Name
        {
            get => _name;
            set => _name = value.Trim();
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => _description = value?.Trim();
        }
        public int UserType { get; set; }
        public int PermissionInWeb { get; set; }
        public List<string?> PermissionKeys { get; set; } = new();
        public List<string?> PermissionKeysRemove { get; set; } = new();
    }
}
