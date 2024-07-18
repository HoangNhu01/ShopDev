using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using System.Text.Json;

namespace ShopDev.Order.ApplicationServices.OrderModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        public OrderService(ILogger<OrderService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public void Create(OrderCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
        }

        public OrderDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            return new();
        }
    }
}
