using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Inventory.Infrastructure.Persistence;

namespace ShopDev.Inventory.ApplicationServices.Common
{
    public abstract class InventoryServiceBase : ServiceBase<InventoryDbContext>
    {
        protected InventoryServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        protected InventoryServiceBase(
            ILogger logger,
            IHttpContextAccessor httpContext,
            InventoryDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper) { }
    }
}
