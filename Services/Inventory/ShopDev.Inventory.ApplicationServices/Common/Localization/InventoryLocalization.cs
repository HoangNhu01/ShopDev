using ShopDev.Common.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Authentication.ApplicationServices.Common.Localization
{
    public class InventoryLocalization : ShopDevLocalization
    {
        public InventoryLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Inventory.ApplicationServices.Common.Localization.SourceFiles");
        }
    }
}
