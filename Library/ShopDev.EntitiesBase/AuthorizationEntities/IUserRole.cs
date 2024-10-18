using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.EntitiesBase.AuthorizationEntitiesAuthorizationEntities
{
    public interface IUserRole<TUserId, TRoleId> : IEntity<int>, IFullAudited
    {
        TUserId UserId { get; set; }
        TRoleId RoleId { get; set; }
    }
}
