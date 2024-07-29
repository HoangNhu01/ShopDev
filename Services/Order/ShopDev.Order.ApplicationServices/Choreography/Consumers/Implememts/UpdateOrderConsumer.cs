using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopDev.Constants.RabbitMQ;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Abstracts;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.RabbitMQ;
using ShopDev.RabbitMQ.Configs;
using System.Text.Json;

namespace ShopDev.Order.ApplicationServices.Choreography.Consumers.Implememts
{
    public class UpdateOrderConsumer : ConsumerService, IUpdateOrderConsumer
    {
        private readonly ILogger<UpdateOrderConsumer> _logger;

        public UpdateOrderConsumer(
            IOptions<RabbitMqConfig> config,
            ILogger<UpdateOrderConsumer> logger
        )
            : base(config, RabbitQueues.UpdateOrder)
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
            _model.QueueBind(_queueName, RabbitExchangeNames.InventoryDirect, "update_order");
            _model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        protected override void ReceiveMessage(object sender, BasicDeliverEventArgs basic)
        {
            try
            {
                var body = basic.Body.ToArray();
                var obj = JsonSerializer.Deserialize<UpdateOrderMessageDto>(body);
                if (obj is not null)
                {
                    BackgroundJob.Enqueue<IOrderService>(x => x.UpdateOrderEvent(null, obj));
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
