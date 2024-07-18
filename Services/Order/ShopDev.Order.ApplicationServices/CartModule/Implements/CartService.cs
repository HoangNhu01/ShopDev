using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.Order.ApplicationServices.CartModule.Abstract;
using ShopDev.Order.ApplicationServices.Common;

namespace ShopDev.Order.ApplicationServices.CartModule.Implements
{
    public class CartService : OrderServiceBase, ICartService
    {
        public CartService(ILogger<CartService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }
    }
}
