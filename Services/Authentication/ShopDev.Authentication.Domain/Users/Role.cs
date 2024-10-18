using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.Domain.Auth.Role;
using ShopDev.Constants.Users;
using ShopDev.EntitiesBase.AuthorizationEntitiesAuthorizationEntities;

namespace ShopDev.Authentication.Domain.Users
{
    [Table(nameof(Role))]
    [Index(
        nameof(Name),
        nameof(PermissionInWeb),
        nameof(Status),
        nameof(Deleted),
        Name = $"IX_{nameof(Role)}"
    )]
    public class Role : IRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// <see cref="UserTypes"/>
        /// </summary>
        public int UserType { get; set; }

        [MaxLength(1024)]
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái A/D
        /// </summary>
        /// <see cref="Constants.Domain.Auth.Role.RoleStatus"/>
        public int Status { get; set; }

        /// <summary>
        /// Quyền thuộc Web nào
        /// <see cref="PermissionInWebs"/>
        /// </summary>
        public int PermissionInWeb { get; set; }
        public virtual List<UserRole> UserRoles { get; } = [];
        public virtual List<RolePermission> RolePermissions { get; } = [];
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
