using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.EntitiesBase.AuthorizationEntities
{
    public interface IRolePermission<TRoleId> : IEntity<int>, ICreatedBy
    {
        TRoleId RoleId { get; set; }
        string PermissionKey { get; set; }
    }
}
