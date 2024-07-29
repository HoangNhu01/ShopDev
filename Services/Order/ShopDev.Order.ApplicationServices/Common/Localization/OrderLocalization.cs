using ShopDev.Common.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Order.ApplicationServices.Common.Localization
{
    public class OrderLocalization : ShopDevLocalization
    {
        public OrderLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Order.ApplicationServices.Common.Localization.SourceFiles");
        }
    }
}
