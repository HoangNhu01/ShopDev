using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.EntitiesBase.AuthorizationEntitiesAuthorizationEntities
{
    public interface IRole : IEntity<int>, IFullAudited
    {
        string Name { get; set; }
        string? Description { get; set; }
    }
}
