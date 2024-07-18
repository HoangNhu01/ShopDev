using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.ApplicationServices.Common;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Infrastructure.Extensions;
using ShopDev.Utils.Linq;
using System.Text.Json;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Implements
{
    public class ProductService : InventoryServiceBase, IProductService
    {
        private readonly ExtensionsDbContext _extensionsDbContext;

        public ProductService(
            ILogger<ProductService> logger,
            IHttpContextAccessor httpContext,
            ExtensionsDbContext extensionsDbContext
        )
            : base(logger, httpContext)
        {
            _extensionsDbContext = extensionsDbContext;
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

        public void Update(ProductUpdateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
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
                        Id = x.Id.ToString(),
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
            _logger.LogInformation(
                $"{nameof(FindById)}: input = {JsonSerializer.Serialize(input)}"
            );
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
