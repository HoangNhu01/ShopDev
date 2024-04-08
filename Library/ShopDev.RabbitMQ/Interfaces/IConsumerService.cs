namespace ShopDev.RabbitMQ.Interfaces
{
    public interface IConsumerService : IDisposable
    {
        Task ReadMessages();
    }
}
