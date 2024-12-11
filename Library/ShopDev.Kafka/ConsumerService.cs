using Microsoft.Extensions.Options;
using ShopDev.Kafka.Configs;
using ShopDev.Kafka.Interfaces;

namespace ShopDev.RabbitMQ
{
    public abstract class ConsumerService : IConsumerService
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ReadMessages() { }
    }
}
