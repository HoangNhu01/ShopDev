using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Abstracts;

namespace ShopDev.Inventory.API.HostedServices
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly IUpdateStockConsumer _updateStockConsumer;

        public ConsumerHostedService(IUpdateStockConsumer updateStockConsumer)
        {
            _updateStockConsumer = updateStockConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             _updateStockConsumer.ReadMessages();
            await Task.CompletedTask;
        }
    }
}
