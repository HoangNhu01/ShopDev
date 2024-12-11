using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.Kafka.Configs
{
    public static class KafkaStartUp
    {
        public static void ConfigureRabbitMQ(this WebApplicationBuilder builder)
        {
			builder.Services.Configure<KafkaConfig>(
				builder.Configuration.GetSection("Kafka")
			);
			builder.Services.AddMassTransit(config =>
            {
                config.UsingInMemory();

                config.AddRider(rider =>
                {
                    rider.AddConsumers(Assembly.GetExecutingAssembly());

                    rider.UsingKafka(
                        (context, k) =>
                        {
                            k.Host("localhost:9092");

                            // Đăng ký các consumer của service
                        }
                    );
                });
            });
        }
    }
}
