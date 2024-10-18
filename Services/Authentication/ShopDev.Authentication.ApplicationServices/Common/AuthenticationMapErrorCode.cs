using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public class AuthenticationMapErrorCode : MapErrorCodeBase<AuthenticationErrorCode>
    {
        public AuthenticationMapErrorCode(
            LocalizationBase localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
