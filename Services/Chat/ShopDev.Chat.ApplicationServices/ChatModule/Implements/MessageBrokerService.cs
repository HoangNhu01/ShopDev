using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MessageBrokerService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "chat_queue";

    public MessageBrokerService()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "user",
            Password = "123abc",
            Port = 5672,
            VirtualHost = "/"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public void SendMessage(object message)
    {
        string body = JsonSerializer.Serialize(message);
        var bodyByte = Encoding.UTF8.GetBytes(body);
        _channel.BasicPublish(
            exchange: "CommunityHub",
            routingKey: QueueName,
            basicProperties: null,
            body: bodyByte
        );
    }

    public void ReceiveMessage(Action<string> onMessageReceived)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            onMessageReceived(message);
        };
        _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
    }
}
