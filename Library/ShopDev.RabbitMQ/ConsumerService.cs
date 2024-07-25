using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopDev.RabbitMQ.Configs;
using ShopDev.RabbitMQ.Interfaces;

namespace ShopDev.RabbitMQ
{
    public abstract class ConsumerService : RabbitMqService, IConsumerService
    {
        protected IModel _model = null!;
        protected IConnection _connection = null!;
        protected string _queueName = null!;

        public ConsumerService(IOptions<RabbitMqConfig> config, string queueName)
            : base(config)
        {
            _queueName = queueName;
        }

        public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }

        public void ReadMessages()
        {
            AsyncEventingBasicConsumer consumer = new(_model);
            consumer.Received += async (model, ea) =>
            {
                ReceiveMessage(model, ea);
                await Task.CompletedTask;
            };
            _model.BasicConsume(_queueName, false, consumer);
        }

        protected abstract void ReceiveMessage(object sender, BasicDeliverEventArgs basic);
    }
}
