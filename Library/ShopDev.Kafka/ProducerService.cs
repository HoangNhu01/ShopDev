using System.Runtime.CompilerServices;
using System.Text.Json;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Options;
using ShopDev.Kafka;
using ShopDev.Kafka.Configs;
using ShopDev.Kafka.Interfaces;
using ShopDev.Utils.DataUtils;

namespace ShopDev.RabbitMQ
{
    public class ProducerService : KafkaService, IProducerService
    {
        protected readonly IPublishEndpoint _publishEndpoint;

        public ProducerService(IPublishEndpoint publishEndpoint, IOptions<KafkaConfig> options)
            : base(options)
        {
            _publishEndpoint = publishEndpoint;
        }
         
        public virtual async Task PublishMessageAsync<TMessage>(
            TMessage entity,
            [CallerMemberName] string? callerMethodName = null
        )
            where TMessage : class
        {
            await _publishEndpoint.Publish(
                entity,
                context =>
                {
                    // Thêm metadata cho message
                    context.Headers.Set("MessageId", Guid.NewGuid());
                    context.Headers.Set("Source", callerMethodName);
                    context.Headers.Set("Timestamp", DateTimeUtils.GetDate());
                }
            );
        }
    }
}
