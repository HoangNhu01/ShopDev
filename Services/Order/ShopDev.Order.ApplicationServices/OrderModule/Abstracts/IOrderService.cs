using Hangfire.Server;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;

namespace ShopDev.Order.ApplicationServices.OrderModule.Abstracts
{
    public interface IOrderService
    {
        Task Create(OrderCreateDto request);
        OrderDetailDto FindById(int id);
        Task ExecuteUpdateStock(PerformContext? context);
        Task UpdateOrderEvent(PerformContext? context, UpdateOrderMessageDto input);

    }
}
