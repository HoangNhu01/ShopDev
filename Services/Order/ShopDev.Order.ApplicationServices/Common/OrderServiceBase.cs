using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.Order.Infrastructure.Persistence;

namespace ShopDev.Order.ApplicationServices.Common
{
    public abstract class OrderServiceBase : ServiceBase<OrderDbContext>
    {
        protected OrderServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

    }
}
