using ShopDev.RabbitMQ.Configs;
using ShopDev.RabbitMQ.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

        public async Task ReadMessages()
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += ReceiveMessage;
            _model.BasicConsume(_queueName, false, consumer);
            await Task.CompletedTask;
        }

        protected abstract Task ReceiveMessage(object sender, BasicDeliverEventArgs basic);
    }
}
