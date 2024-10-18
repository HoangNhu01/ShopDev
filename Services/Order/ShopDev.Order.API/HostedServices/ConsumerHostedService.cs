using ShopDev.Order.ApplicationServices.Choreography.Consumers.Abstracts;

namespace ShopDev.Order.API.HostedServices
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly IUpdateOrderConsumer _updateOrderConsumer;

        public ConsumerHostedService(IUpdateOrderConsumer updateOrderConsumer)
        {
            _updateOrderConsumer = updateOrderConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updateOrderConsumer.ReadMessages();
            await Task.CompletedTask;
        }
    }
}
