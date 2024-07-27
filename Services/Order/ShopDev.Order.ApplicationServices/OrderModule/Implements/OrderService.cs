using System.Text.Json;
using AutoMapper;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
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
using ShopDev.Order.Domain.Order;
using ShopDev.Order.Domain.Products;
using ShopDev.Order.Infrastructure.Persistence;
using ShopDev.Utils.DataUtils;

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
            var order = await _dbContext.Orders.AddAsync(
                new()
                {
                    Id = Guid.NewGuid(),
                    ShipAddress = input.ShipAddress,
                    ShipEmail = input.ShipEmail,
                    ShipName = input.ShipName,
                    ShipPhoneNumber = input.ShipPhoneNumber,
                    OrderDate = DateTimeUtils.GetDate(),
                }
            );
            order.Entity.OrderDetails.AddRange(
                input.CartItems.Select(x => new OrderDetail
                {
                    ProductId = x.Id,
                    Product = new()
                    {
                        Id = x.Id,
                        OrderId = order.Entity.Id,
                        Name = x.Name,
                        ThumbUri = x.ThumbUri,
                        Title = x.Title,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        ShopId = x.ShopId,
                        SpuId = x.SpuId,
                        Spus =
                        [
                            .. x.Spus.Select(e => new Spu { Name = e.Name, Options = e.Options })
                        ]
                    }
                })
            );
            await _dbContext.SaveChangesOutBoxAsync<Product>();
        }

        [AutomaticRetry(Attempts = 0)]
        [HangfireLogEverything]
        public async Task ExecuteUpdateStock(PerformContext? context)
        {
            _logger.LogInformation($"{nameof(ExecuteUpdateStock)}");
            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                var transaction = _dbContext.Database.BeginTransaction();
                List<OutboxMessage> messages = await _dbContext
                    .Set<OutboxMessage>()
                    .FromSqlRaw(
                        @"
                            UPDATE TOP (@count) [sd_order].[OutboxMessage]
                            SET IsLock = 1
                            OUTPUT INSERTED.*
                            WHERE ProcessedOnUtc is null and IsLock = @isLock
                        ",
                        new SqlParameter("@count", 20),
                        new SqlParameter("@isLock", false)
                    )
                    .ToListAsync();
                if (messages.Count == 0)
                {
                    BackgroundJob.Delete(context!.BackgroundJob.Id);
                    return;
                }
                messages
                    .AsParallel()
                    .ForAll(outboxMessage =>
                    {
                        var outbox = _dbContext.Attach(outboxMessage).Entity;
                        try
                        {
                            var updateStocks = JsonSerializer.Deserialize<
                                List<UpdateStockMessageDto>
                            >(outbox.Content);
                            _updateStockProducer.PublishMessage(
                                updateStocks,
                                exchangeName: RabbitExchangeNames.InventoryDirect,
                                bindingKey: "update_stock"
                            );

                            outbox.ProcessedOnUtc = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(
                                $"{nameof(ExecuteUpdateStock)}: error = {ex.Message} "
                            );
                            outbox.Error = ex.Message;
                        }
                    });
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        public OrderDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            return new();
        }
    }
}
