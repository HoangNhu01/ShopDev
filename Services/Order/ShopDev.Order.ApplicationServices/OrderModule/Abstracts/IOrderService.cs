using ShopDev.Order.ApplicationServices.OrderModule.Dtos;

namespace ShopDev.Order.ApplicationServices.OrderModule.Abstracts
{
    public interface IOrderService
    {
        Task Create(OrderCreateDto request);
        OrderDetailDto FindById(int id);
    }
}
