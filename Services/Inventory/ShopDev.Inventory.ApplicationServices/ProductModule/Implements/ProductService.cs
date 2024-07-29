using System.Text.Json;
using AutoMapper;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.Domain.Order.OrderGen;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.RabbitMQ;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.InfrastructureBase.Hangfire.Attributes;
using ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Dtos;
using ShopDev.Inventory.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.Inventory.ApplicationServices.Choreography.Producers.Dtos;
using ShopDev.Inventory.ApplicationServices.Common;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Infrastructure.Persistence;
using ShopDev.Utils.Hangfire;
using ShopDev.Utils.Linq;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Implements
{
    public class ProductService : InventoryServiceBase, IProductService
    {
        private readonly IUpdateOrderProducer _updateOrderProducer;

        public ProductService(
            ILogger<ProductService> logger,
            IHttpContextAccessor httpContext,
            InventoryDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IUpdateOrderProducer updateOrderProducer
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper)
        {
            _updateOrderProducer = updateOrderProducer;
        }

        public void Create(ProductCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            HashSet<string> checkDuplicate = [];
            if (input.Variations.Exists(x => !checkDuplicate.Add(x.Name)))
            {
                throw new UserFriendlyException(InventoryErrorCode.VariationIsDuplicate);
            }
            var product = _dbContext
                .Products.Add(
                    new()
                    {
                        Description = input.Description,
                        Name = input.Name,
                        ThumbUri = input.ThumbUri,
                        Title = input.Title,
                        Price = input.Price,
                        ShopId = input.ShopId,
                        IsFeatured = false,
                        Variations =
                        [
                            .. input.Variations.Select(x => new Variation
                            {
                                Name = x.Name,
                                Options = x.Options
                            })
                        ],
                        Attributes =
                        [
                            .. input.Attributes.Select(x => new AttributeType
                            {
                                Name = x.Name,
                                Value = x.Value,
                                AttributeId = Guid.NewGuid()
                            })
                        ],
                        Spus =
                        [
                            .. input.Spus.Select(x => new Spu
                            {
                                Index = x.Index,
                                Price = x.Price,
                                Stock = x.Stock,
                            })
                        ],
                        Categories =
                        [
                            .. input.Categories.Select(x => new CategoryType {
                            CategoryId = x.CategoryId
                        })
                        ]
                    }
                )
                .Entity;
            _dbContext.SaveChanges();
        }

        [AutomaticRetry(Attempts = 5, DelaysInSeconds = [10, 20, 20, 30, 60])]
        [HangfireLogEverything]
        public async Task UpdateStockEvent(
            PerformContext? context,
            List<UpdateStockMessageDto> input
        )
        {
            _logger.LogInformation(
                $"{nameof(UpdateStockEvent)}: input = {JsonSerializer.Serialize(input)}"
            );
            var updateStock = input.ToDictionary(e => e.SpuId);
            var spu = GetEntities<Spu>(expression: x => updateStock.Keys.Contains(x.Id)).ToList();
            Guid orderId = Guid.Empty;
            spu.ForEach(s =>
            {
                var item = updateStock[s.Id];
                orderId = item.OrderId;
                if (s.Stock + item.Quantity >= 0)
                {
                    s.Stock += updateStock[s.Id].Quantity;
                }
            });
            string? message = null;
            int eventType = default;
            try
            {
                eventType = OrderStatuses.Confirmed;
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                var entities = ex.Entries.Select(x => x.Entity);
                foreach (var item in entities)
                {
                    if (item is Spu spuEntry)
                    {
                        var update = updateStock[spuEntry.Id];
                        if (spuEntry.Stock + update.Quantity >= 0)
                        {
                            spuEntry.Stock += updateStock[spuEntry.Id].Quantity;
                        }
                    }
                }
                try
                {
                    eventType = OrderStatuses.Confirmed;
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    int retryCount = HangfireUltils.GetRetryCount(context!.BackgroundJob.Id);
                    if (retryCount == 5)
                    {
                        eventType = OrderStatuses.Canceled;
                        message = L("error_OrderProcessProblemOccurs");
                    }
                }
                finally
                {
                    _updateOrderProducer.PublishMessage(
                        new UpdateOrderMessageDto()
                        {
                            Message = message,
                            EventType = eventType,
                            OrderId = orderId,
                        },
                        exchangeName: RabbitExchangeNames.InventoryDirect,
                        bindingKey: "update_order"
                    );
                }
            }
        }

        public void Update(ProductUpdateDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            HashSet<string> checkDuplicate = [];
            if (input.Variations.Exists(x => !checkDuplicate.Add(x.Name)))
            {
                throw new UserFriendlyException(InventoryErrorCode.VariationIsDuplicate);
            }
            if (input.Spus.Exists(x => !checkDuplicate.Add(string.Join(string.Empty, x.Index))))
            {
                throw new UserFriendlyException(InventoryErrorCode.VariationIsDuplicate);
            }
            checkDuplicate.Clear();
            var product =
                FindEntity<Product>(expression: x => x.Id == input.Id)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            product.Attributes = _mapper.Map<List<AttributeType>>(input.Attributes);
            if (
                input.Variations.ExceptBy(product.Variations.Select(x => x.Name), x => x.Name)
                is not null
            )
            {
                product.Spus.Clear();
            }

            product.Variations = _mapper.Map<List<Variation>>(input.Variations);
            product.Price = input.Price;
            if (input.Spus.Count > 0)
            {
                UpdateItems(
                    product.Spus,
                    input.Spus,
                    (x, y) => x.Id == y.Id,
                    (x, y) =>
                    {
                        x.Index = y.Index;
                        x.Price = y.Price;
                        x.Stock = y.Stock;
                    }
                );
                product.Spus.AddRange(
                    input.Spus.Select(x => new Spu
                    {
                        Index = x.Index,
                        Price = x.Price,
                        Stock = x.Stock
                    })
                );
            }
            _dbContext.SaveChanges();
        }

        public ProductDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var product =
                FindEntity<Product>(
                    expression: x => x.Id == id,
                    include: s => s.Include(x => x.Spus)
                ) ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            return new()
            {
                Description = product.Description,
                Name = product.Name,
                Id = product.Id,
                ShopId = product.ShopId,
                ThumbUri = product.ThumbUri,
                Title = product.Title,
                Attributes =
                [
                    .. product
                        .Attributes.GroupBy(x => x.Name)
                        .Select(x => new AttributeDetailDto
                        {
                            Name = x.Key,
                            Value = [.. x.Select(c => c.Value)]
                        })
                ],
                Spus =
                [
                    .. product.Spus.Select(x => new SpuDetailDto
                    {
                        Id = x.Id,
                        Index = x.Index,
                        Price = x.Price,
                        Stock = x.Stock
                    })
                ],
                Variations =
                [
                    .. product.Variations.Select(x => new VariationDetailDto
                    {
                        Options = x.Options,
                        Name = x.Name
                    })
                ],
                Categories =
                [
                    .. product.Categories.Select(x => new CategoryTypeDetailDto{
                        CategoryId = x.CategoryId,
                        Name = x.Category.Name
                    })
                ]
            };
        }

        public PagingResult<ProductDetailDto> FindAll(ProductFilterDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var product =
                GetIQueryableResult<ProductDetailDto, Product>(
                    selector: x =>
                        new()
                        {
                            Description = x.Description,
                            Name = x.Name,
                            Id = x.Id,
                            ShopId = x.ShopId,
                            ThumbUri = x.ThumbUri,
                            Title = x.Title,
                            Attributes = x.Attributes.GroupBy(x => x.Name)
                                .Select(x => new AttributeDetailDto
                                {
                                    Name = x.Key,
                                    Value = x.Select(c => c.Value).ToList()
                                })
                                .ToList(),
                            Variations = x.Variations.Select(x => new VariationDetailDto
                            {
                                Options = x.Options,
                                Name = x.Name
                            })
                                .ToList()
                        },
                    predicate: x =>
                        (
                            string.IsNullOrEmpty(input.ShopName)
                            || x.Shop.Name.Contains(input.ShopName)
                        )
                        && (
                            string.IsNullOrEmpty(input.ProductName)
                            || x.Shop.Name.Contains(input.ProductName)
                        )
                        && (!input.Price.HasValue || x.Price == input.Price.Value)
                        && (!input.ShopId.HasValue || x.ShopId == input.ShopId.Value)
                        && (
                            input.AttributeNames == null
                            || x.Attributes.Any(s => input.AttributeNames.Contains(s.Value))
                        )
                        && (
                            input.CategoryNames == null
                            || x.Categories.Any(s => input.CategoryNames.Contains(s.Category.Name))
                        )
                //include: x => x.Include(x => x.Shop).Include(x => x.Categories).ThenInclude(x => x.Category)
                ) ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            var result = new PagingResult<ProductDetailDto>
            {
                // đếm tổng trước khi phân trang
                TotalItems = product.Count()
            };
            product = product.OrderDynamic(input.Sort);
            if (input.PageSize != -1)
            {
                product = product.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = product;
            return result;
        }
    }
}
