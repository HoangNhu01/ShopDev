using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Chat.ApplicationServices.Common
{
    public class ChatMapErrorCode : MapErrorCodeBase<ChatErrorCode>
    {
        public ChatMapErrorCode(
            LocalizationBase localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
