using ShopDev.Notification.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.Notification.Configs
{
    public static class NotificationConfigStartUp
    {
        public static void ConfigureNotification(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<NotificationConfig>(
                builder.Configuration.GetSection("Notification")
            );
            builder.Services.AddSingleton<INotificationLocalization, NotificationLocalization>();
            builder.Services.AddSingleton<INotificationMapErrorCode, NotificationMapErrorCode>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
        }
    }
}
