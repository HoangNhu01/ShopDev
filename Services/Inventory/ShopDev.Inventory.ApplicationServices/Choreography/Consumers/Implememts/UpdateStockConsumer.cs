using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopDev.Constants.RabbitMQ;
using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Abstracts;
using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.RabbitMQ;
using ShopDev.RabbitMQ.Configs;

namespace ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Implememts
{
    public class UpdateStockConsumer : ConsumerService, IUpdateStockConsumer
    {
        public UpdateStockConsumer(IOptions<RabbitMqConfig> config)
            : base(config, RabbitQueues.UpdateStock)
        {
            _connection = CreateConnection();
            _model = _connection.CreateModel();
            Dictionary<string, object> queueArgs =
                new()
                {
                    { "x-queue-type", "quorum" } // Đặt loại hàng đợi thành quorum
                };
            _model.QueueDeclare(
                _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _model.ExchangeDeclare(
                RabbitExchangeNames.InventoryDirect,
                ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );
            _model.QueueBind(_queueName, RabbitExchangeNames.InventoryDirect, string.Empty);
            _model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        protected override async Task ReceiveMessage(object sender, BasicDeliverEventArgs basic)
        {
            var body = basic.Body.ToArray();
            Console.WriteLine("123qwe");
            var obj = JsonSerializer.Deserialize<UpdateStockMessageDto>(body);
            if (obj is not null)
            {
                //BackgroundJob.Enqueue<IOrderService>(x => x.BackgroundCreateOrderPayment(obj, null));
            }
            await Task.CompletedTask;
            _model.BasicAck(basic.DeliveryTag, false);
        }
    }
}
