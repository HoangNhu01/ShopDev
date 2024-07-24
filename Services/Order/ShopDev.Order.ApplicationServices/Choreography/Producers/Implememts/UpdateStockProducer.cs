using System.Text.Json;
using Microsoft.Extensions.Options;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.RabbitMQ;
using ShopDev.RabbitMQ.Configs;

namespace ShopDev.Order.ApplicationServices.Choreography.Producers.Implememts
{
    public class UpdateStockProducer : ProducerService, IUpdateStockProducer
    {
        public UpdateStockProducer(IOptions<RabbitMqConfig> config)
            : base(config)
        {
            _connection = CreateConnection();
            _model = _connection.CreateModel();
        }

        public override void PublishMessage<TEntity>(
            TEntity? entity,
            string exchangeName,
            string bindingKey
        )
            where TEntity : class
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(entity);
            _model.BasicPublish(exchangeName, bindingKey, true, null, body);
        }
    }
}
