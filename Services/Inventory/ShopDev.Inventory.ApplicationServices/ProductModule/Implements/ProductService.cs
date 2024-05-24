using System.Collections.Generic;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Infrastructure.Extensions;

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
                        Variations =
                        [
                            ..input
                            .Variations.Select(x => new Variation
                            {
                                Name = x.Name,
                                Options = x.Options
                            })
                        ],
                        Attributes =
                        [
                            .. input
					        .Attributes.Select(x => new AttributeType
					        {
						        Name = x.Name,
						        Value = x.Value,
						        AttributeId = Guid.NewGuid()
					        })
                        ],
                        Spus =
                        [
                            .. input
					        .Spus.Select(x => new Spu
					        {
						        Index = x.Index,
						        Price = x.Price,
						        Stock = x.Stock
					        })
                        ]
                    }
                )
                .Entity;
            _dbContext.SaveChanges();
        }

        public ProductDetailDto FindById(string id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Id == id);

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
                    .. product.Attributes.GroupBy(x => x.Name).Select(x => new AttributeDetailDto 
                    { 
                        Name = x.Key, 
                        Value = [.. x.Select(c => c.Value)] 
                    })
                ],
                Spus =
                [
                    .. product.Spus.Select(x => new SpuDetailDto { Id = x.Id, Index = x.Index, Price = x.Price, Stock = x.Stock })
                ]
            };
        }
    }
}
