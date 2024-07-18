using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Order.ApplicationServices.Common
{
    public class OrderMapErrorCode : MapErrorCodeBase<InventoryErrorCode>
    {
        public OrderMapErrorCode(
            LocalizationBase localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
