using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Common.Localization
{
    public class ShopDevLocalization : LocalizationBase
    {
        public ShopDevLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Common.Localization.SourceFiles");
        }
    }
}
