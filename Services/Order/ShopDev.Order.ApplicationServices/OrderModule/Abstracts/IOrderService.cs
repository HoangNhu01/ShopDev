using Hangfire.Server;
using ShopDev.ApplicationBase.Common;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;

namespace ShopDev.Order.ApplicationServices.OrderModule.Abstracts
{
    public interface IOrderService
    {
        Task Create(OrderCreateDto request);
        Task<OrderDto> FindById(Guid id);
        Task ExecuteUpdateStock(PerformContext? context);
        Task UpdateOrderEvent(PerformContext? context, UpdateOrderMessageDto input);
        PagingResult<OrderDto> FindAll(OrderFilterDto input);
    }
}
