using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public class InventoryMapErrorCode : MapErrorCodeBase<InventoryErrorCode>
    {
        public InventoryMapErrorCode(
            LocalizationBase localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
