using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ShopDev.RabbitMQ.Configs;
using ShopDev.RabbitMQ.Interfaces;

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
            where TEntity : class
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(entity);
            //Kiểm tra queue và exchange còn tồn tại hay không
            _model.ExchangeDeclarePassive(exchangeName);
            _model.BasicPublish(exchangeName, bindingKey, true, null, body);
        }
    }
}
