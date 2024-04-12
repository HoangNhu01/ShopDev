using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.EntitiesBase.AuthorizationEntities
{
    public interface IUser : IEntity<int>, IFullAudited
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
    }
}
