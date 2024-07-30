using System.Text.Json;
using AutoMapper;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.RabbitMQ;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.InfrastructureBase.Hangfire.Attributes;
using ShopDev.InfrastructureBase.Persistence.OutBox;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Dtos;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.Order.Domain.Order;
using ShopDev.Order.Domain.Products;
using ShopDev.Order.Infrastructure.Persistence;
using ShopDev.Utils.Cache;
using ShopDev.Utils.DataUtils;
using ShopDev.Utils.Linq;
using StackExchange.Redis;

namespace ShopDev.Order.ApplicationServices.OrderModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        private readonly IUpdateStockProducer _updateStockProducer;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public OrderService(
            ILogger<OrderService> logger,
            IHttpContextAccessor httpContext,
            OrderDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IUpdateStockProducer updateStockProducer,
            IConnectionMultiplexer connectionMultiplexer
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper)
        {
            _updateStockProducer = updateStockProducer;
            _connectionMultiplexer = connectionMultiplexer;
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
                            var updateStocks =
                                JsonSerializer.Deserialize<List<UpdateStockMessageDto>>(
                                    outbox.Content
                                ) ?? throw new ArgumentException("Json convert error");
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

        public async Task<OrderDto> FindById(Guid id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var order =
                await FindEntityAsync<OrderGen>(
                    s => s.Id == id,
                    include: x => x.Include(s => s.OrderDetails)
                ) ?? throw new UserFriendlyException(OrderErrorCode.OrderNotFound);
            return _mapper.Map<OrderDto>(order);
        }

        [AutomaticRetry(Attempts = 5, DelaysInSeconds = [10, 20, 20, 30, 60])]
        [HangfireLogEverything]
        public async Task UpdateOrderEvent(PerformContext? context, UpdateOrderMessageDto input)
        {
            var lockKey = $"order-lock-key-{input.OrderId}";
            var lockValue = Guid.NewGuid().ToString();
            var lockExpiration = TimeSpan.FromSeconds(30);
            var db = _connectionMultiplexer.GetDatabase();
            if (await db.AcquireLockAsync(lockKey, lockValue, lockExpiration))
            {
                try
                {
                    // Thực hiện công việc của bạn ở đây
                    var order =
                        await FindEntityAsync<OrderGen>(
                            x => x.Id == input.OrderId,
                            include: x => x.Include(s => s.OrderDetails)
                        ) ?? throw new UserFriendlyException(OrderErrorCode.OrderNotFound);

                    order.Status = input.EventType;
                    await _dbContext.SaveChangesAsync();
                }
                finally
                {
                    await db.ReleaseLockAsync(lockKey, lockValue);
                }
            }
            else
            {
                throw new UserFriendlyException(ErrorCode.InternalServerError);
            }
        }

        public PagingResult<OrderDto> FindAll(OrderFilterDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var orderQuery =
                GetIQueryableResult<OrderDto, OrderGen>(
                    selector: x =>
                        new()
                        {
                            ShipAddress = x.ShipAddress,
                            Id = x.Id,
                            ShipName = x.ShipName,
                            OrderDate = x.OrderDate,
                            ShipEmail = x.ShipEmail,
                            ShipPhoneNumber = x.ShipPhoneNumber,
                            PaymentStatus = x.PaymentStatus,
                            Status = x.Status,
                            TotalPrice = x.TotalPrice,
                            UserId = x.UserId,
                            OrderDetails = x.OrderDetails.Select(s => new OrderDetailDto
                            {
                                Id = s.Id,
                                StockStatus = s.StockStatus,
                                ProductId = s.ProductId,
                            })
                                .ToList(),
                        },
                    predicate: x =>
                        (
                            string.IsNullOrEmpty(input.ShipPhoneNumber)
                            || x.ShipPhoneNumber.Contains(input.ShipPhoneNumber)
                        )
                        && (
                            string.IsNullOrEmpty(input.ShipAddress)
                            || x.ShipAddress.Contains(input.ShipAddress)
                        )
                        && (!input.Status.HasValue || x.Status == input.Status.Value)
                        && (
                            !input.PaymentStatus.HasValue
                            || x.PaymentStatus == input.PaymentStatus.Value
                        )
                        && (
                            string.IsNullOrEmpty(input.ShipEmail)
                            || x.ShipEmail.Contains(input.ShipEmail)
                        )
                        && (
                            string.IsNullOrEmpty(input.ShipName)
                            || x.ShipName.Contains(input.ShipName)
                        )
                //include: x => x.Include(x => x.Shop).Include(x => x.Categories).ThenInclude(x => x.Category)
                ) ?? throw new UserFriendlyException(OrderErrorCode.OrderNotFound);
            var result = new PagingResult<OrderDto>
            {
                // đếm tổng trước khi phân trang
                TotalItems = orderQuery.Count()
            };
            orderQuery = orderQuery.OrderDynamic(input.Sort);
            if (input.PageSize != -1)
            {
                orderQuery = orderQuery.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = orderQuery;
            return result;
        }
    }
}
