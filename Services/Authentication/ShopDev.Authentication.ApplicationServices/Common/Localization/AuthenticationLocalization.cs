using ShopDev.Common.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Authentication.ApplicationServices.Common.Localization
{
    public class AuthenticationLocalization : ShopDevLocalization
    {
        public AuthenticationLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Authentication.ApplicationServices.Common.Localization.SourceFiles");
        }
    }
}
