using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.Notification.Exceptions
{
    public class NotificationException : BaseException
    {
        public NotificationException(int errorCode)
            : base(errorCode) { }

        public NotificationException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
