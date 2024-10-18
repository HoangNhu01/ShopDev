namespace ShopDev.Notification.Configs
{
    public class NotificationConfig
    {
        public const string SendNotificationPath = "/api/notification/notification/send-system";
        public string BaseUrl { get; set; } = null!;
    }
}
