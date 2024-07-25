using System.Text.Json;
using AutoMapper;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.RabbitMQ;
using ShopDev.InfrastructureBase.Hangfire.Attributes;
using ShopDev.InfrastructureBase.Persistence.OutBox;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Dtos;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Implememts;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.Order.Domain.Products;
using ShopDev.Order.Infrastructure.Persistence;

namespace ShopDev.Order.ApplicationServices.OrderModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        private readonly IUpdateStockProducer _updateStockProducer;

        public OrderService(
            ILogger<OrderService> logger,
            IHttpContextAccessor httpContext,
            OrderDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IUpdateStockProducer updateStockProducer
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper)
        {
            _updateStockProducer = updateStockProducer;
        }

        public async Task Create(OrderCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            List<OutboxMessage> messages = await _dbContext
                .Set<OutboxMessage>()
                .Where(m => !m.ProcessedOnUtc.HasValue)
                .Take(20)
                .ToListAsync();

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
                    Product = new Product()
                    {
                        Name = "Quần áo",
                        ThumbUri = "123",
                        Title = "Quần áo"
                    },
                }
            );
            await _dbContext.SaveChangesOutBoxAsync<Product>();
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = [10, 20, 20])]
        [HangfireLogEverything]
        public async Task ExecuteUpdateStock(PerformContext? context)
        {
            List<OutboxMessage> messages = await _dbContext
                .Set<OutboxMessage>()
                .Where(m => !m.ProcessedOnUtc.HasValue)
                .Take(20)
                .ToListAsync();
            if (messages.Count == 0)
            {
                return;
            }
            messages
                .AsParallel()
                .ForAll(outboxMessage =>
                {
                    var updateStocks = JsonSerializer.Deserialize<List<UpdateStockMessageDto>>(
                        outboxMessage.Content
                    );
                    _updateStockProducer.PublishMessage(
                        updateStocks,
                        exchangeName: RabbitExchangeNames.InventoryDirect,
                        bindingKey: string.Empty
                    );

                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                });

            await _dbContext.SaveChangesAsync();
        }

        public OrderDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            return new();
        }
    }
}
