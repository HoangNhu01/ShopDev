using System.Text.Json;
using Microsoft.Extensions.Options;
using ShopDev.Constants.RabbitMQ;
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

    }
}
