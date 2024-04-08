using ShopDev.RabbitMQ.Configs;
using ShopDev.RabbitMQ.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ShopDev.RabbitMQ
{
    public class ProducerService : RabbitMqService, IProducerService
    {
        protected IModel _model = null!;
        protected IConnection _connection = null!;

        public ProducerService(IOptions<RabbitMqConfig> options)
            : base(options) { }

        public virtual void PublishMessage<TEntity>(
            TEntity? entity,
            string exchangeName,
            string bindingKey
        )
            where TEntity : class { }
    }
}
