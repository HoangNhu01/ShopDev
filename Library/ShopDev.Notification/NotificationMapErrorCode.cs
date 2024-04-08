using ShopDev.ApplicationBase.Localization;
using ShopDev.Notification.Constants;
using ShopDev.Notification.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.Notification
{
    public class NotificationMapErrorCode
        : MapErrorCodeBase<NotificationErrorCode>,
            INotificationMapErrorCode
    {
        protected override string PrefixError => "error_notification_";

        public NotificationMapErrorCode(
            INotificationLocalization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
