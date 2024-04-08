using ShopDev.RabbitMQ.Configs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ShopDev.RabbitMQ
{
    public abstract class RabbitMqService
    {
        private readonly RabbitMqConfig _config;

        protected RabbitMqService(IOptions<RabbitMqConfig> options)
        {
            _config = options.Value;
        }

        public virtual IConnection CreateConnection()
        {
            return _config.CreateConnection(true);
        }
    }
}
