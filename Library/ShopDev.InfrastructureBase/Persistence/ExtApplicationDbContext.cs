using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MongoDB.Entities;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Utils.DataUtils;

namespace ShopDev.InfrastructureBase.Persistence
{
    public class ExtApplicationDbContext : DBContext
    {
        protected readonly IHttpContextAccessor _httpContextAccessor = null!;
        protected readonly int? UserId = null;

        public ExtApplicationDbContext() { }

        public ExtApplicationDbContext(IHttpContextAccessor httpContextAccessor)
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
                if (f.GetType().GetProperty("Id")?.GetValue(f) is null)
                {
                    f.CreatedBy = UserId;
                    f.CreatedDate = DateTimeUtils.GetDate();
                }
                if (
                    bool.TryParse(
                        $"{f.GetType().GetProperty("Deleted")?.GetValue(f)}",
                        out bool deleted
                    )
                    && deleted
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
