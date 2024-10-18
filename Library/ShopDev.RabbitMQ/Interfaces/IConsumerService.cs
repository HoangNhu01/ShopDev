namespace ShopDev.RabbitMQ.Interfaces
{
    public interface IConsumerService : IDisposable
    {
        void ReadMessages();
    }
}
