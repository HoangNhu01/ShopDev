using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Users;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto
{
    public class RoleDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên Role
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// <see cref="UserTypes"/>
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
        public int PermissionInWeb { get; set; }
        public int Status { get; set; }
        public int TotalUse { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedByUserName { get; set; }
        public List<RolePermission> PermissionDetails { get; set; } = new();
    }
}
