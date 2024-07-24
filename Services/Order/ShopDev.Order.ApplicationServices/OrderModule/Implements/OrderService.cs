using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.Order.Domain.Order;

namespace ShopDev.Order.ApplicationServices.OrderModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        public OrderService(ILogger<OrderService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public async Task Create(OrderCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            var order = await _dbContext.Orders.AddAsync(
                new()
                {
                    Id = Guid.NewGuid(),
                    ShipAddress = "123",
                    ShipEmail = "asd@gmail.com",
                    ShipName = "admin",
                    ShipPhoneNumber = "01234412233",
                    OrderDate = DateTime.Now,
                }
            );
            order.Entity.OrderDetails.Add(
                new()
                {
                    ProductId = 1,
                    SpuId = 1,
                    Product = new Domain.Products.Product()
                    {
                        Name = "Quần áo",
                        ThumbUri = "123",
                        Title = "Quần áo"
                    }
                }
            );
            await _dbContext.SaveChangesOutBoxAsync<OrderGen>();
        }

        public OrderDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            return new();
        }
    }
}
