using System.Text.Json;
using Microsoft.Extensions.Options;
using ShopDev.Constants.RabbitMQ;
using ShopDev.Inventory.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.RabbitMQ;
using ShopDev.RabbitMQ.Configs;

namespace ShopDev.Inventory.ApplicationServices.Choreography.Producers.Implememts
{
    public class UpdateOrderProducer : ProducerService, IUpdateOrderProducer
    {
        public UpdateOrderProducer(IOptions<RabbitMqConfig> config)
            : base(config)
        {
            _connection = CreateConnection();
            _model = _connection.CreateModel();
        }
    }
}
