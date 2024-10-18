using ShopDev.Common.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Chat.ApplicationServices.Common.Localization
{
    public class ChatLocalization : ShopDevLocalization
    {
        public ChatLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Chat.ApplicationServices.Common.Localization.SourceFiles");
        }
    }
}
