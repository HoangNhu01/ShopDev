using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Order.Infrastructure.Persistence;

namespace ShopDev.Order.ApplicationServices.Common
{
    public abstract class OrderServiceBase : ServiceBase<OrderDbContext>
    {
        protected OrderServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        protected OrderServiceBase(
            ILogger logger,
            IHttpContextAccessor httpContext,
            OrderDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper) { }
    }
}
