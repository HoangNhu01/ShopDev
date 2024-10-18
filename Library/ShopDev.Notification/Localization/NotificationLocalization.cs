using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Notification.Localization
{
    public class NotificationLocalization : LocalizationBase, INotificationLocalization
    {
        public NotificationLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.Notification.Localization.SourceFiles");
        }
    }
}
