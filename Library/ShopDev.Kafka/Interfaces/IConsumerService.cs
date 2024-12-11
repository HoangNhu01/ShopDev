namespace ShopDev.Kafka.Interfaces
{
    public interface IConsumerService : IDisposable
    {
        void ReadMessages();
    }
}
