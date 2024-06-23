using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Entities;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Utils.DataUtils;

namespace ShopDev.InfrastructureBase.Persistence
{
    public class ApplicationMongoDbContext : DBContext
    {
        protected readonly IHttpContextAccessor _httpContextAccessor = null!;
        protected readonly int? UserId = null;

        public ApplicationMongoDbContext(
            IHttpContextAccessor httpContextAccessor,
            string database,
            MongoClientSettings settings,
            ModifiedBy? modifiedBy = null
        )
            : base(database, settings, modifiedBy)
        {
            _httpContextAccessor = httpContextAccessor;
            var claims = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                UserId = userId;
            }
        }

        public ApplicationMongoDbContext(
            IHttpContextAccessor httpContextAccessor,
            string database,
            string host = "127.0.0.1",
            int port = 27017,
            ModifiedBy? modifiedBy = null
        )
            : base(database, host, port, modifiedBy)
        {
            _httpContextAccessor = httpContextAccessor;
            var claims = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                UserId = userId;
            }
        }

        protected override Action<TEntity>? OnBeforeSave<TEntity>()
        {
            Action<IFullAudited> action = f =>
            {
                if (f.GetType().GetProperty("ID")?.GetValue(f) is null)
                {
                    f.CreatedBy = 1;
                    f.CreatedDate = DateTimeUtils.GetDate();
                }
                else if (
                    bool.TryParse(
                        $"{f.GetType().GetProperty("Deleted")?.GetValue(f)}",
                        out bool deleted
                    ) && deleted
                )
                {
                    f.DeletedBy = UserId;
                    f.DeletedDate = DateTimeUtils.GetDate();
                }
                else
                {
                    f.ModifiedBy = UserId;
                    f.ModifiedDate = DateTimeUtils.GetDate();
                }
            };

            return action as Action<TEntity>;
        }
    }
}
