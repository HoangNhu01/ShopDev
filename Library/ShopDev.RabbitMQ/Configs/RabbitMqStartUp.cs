using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.RabbitMQ.Configs
{
    public static class RabbitMqStartUp
    {
        public static void ConfigureRabbitMQ(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMqConfig>(
                builder.Configuration.GetSection("RabbitMQ")
            );
        }
    }
}
