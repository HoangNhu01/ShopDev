using System.Text;
using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopDev.Constants.RabbitMQ;
using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Abstracts;
using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.RabbitMQ;
using ShopDev.RabbitMQ.Configs;
using ShopDev.RabbitMQ.Interfaces;

namespace ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Implememts
{
    public class UpdateStockConsumer : ConsumerService, IUpdateStockConsumer
    {
        private readonly ILogger<UpdateStockConsumer> _logger;

        public UpdateStockConsumer(
            IOptions<RabbitMqConfig> config,
            ILogger<UpdateStockConsumer> logger
        )
            : base(config, RabbitQueues.UpdateStock)
        {
            _logger = logger;
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
                arguments: queueArgs
            );
            _model.ExchangeDeclare(
                RabbitExchangeNames.InventoryDirect,
                ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );
            _model.QueueBind(_queueName, RabbitExchangeNames.InventoryDirect, "update_stock");
            _model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        protected override void ReceiveMessage(object sender, BasicDeliverEventArgs basic)
        {
            try
            {
                var body = basic.Body.ToArray();
                var obj = JsonSerializer.Deserialize<List<UpdateStockMessageDto>>(body);
                if (obj is not null)
                {
                    BackgroundJob.Enqueue<IProductService>(x => x.UpdateStockEvent(null, obj));
                }
                _model.BasicAck(basic.DeliveryTag, false);
            }
            catch
            {
                _logger.LogError($"{nameof(ReceiveMessage)}: Error processing message");

                // Optionally requeue the message for retry
                // Set requeue to false if you want to handle it as a dead-letter
                _model.BasicNack(basic.DeliveryTag, false, requeue: false);
            }
        }
    }
}
