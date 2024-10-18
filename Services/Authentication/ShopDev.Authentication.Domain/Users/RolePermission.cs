using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.RolePermission;
using ShopDev.EntitiesBase.AuthorizationEntities;

namespace ShopDev.Authentication.Domain.Users
{
    [Table(nameof(RolePermission))]
    [Index(nameof(PermissionKey), nameof(RoleId), Name = $"IX_{nameof(RolePermission)}")]
    public class RolePermission : IRolePermission<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Tên permission <br/>
        /// <see cref="PermissionConfig"/>
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string PermissionKey { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
