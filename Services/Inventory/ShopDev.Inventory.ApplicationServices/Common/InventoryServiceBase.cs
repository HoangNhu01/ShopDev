using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.Inventory.Infrastructure.Persistence;

namespace ShopDev.Inventory.ApplicationServices.Common
{
    public abstract class InventoryServiceBase : ServiceBase<InventoryDbContext>
    {
        protected InventoryServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

    }
}
