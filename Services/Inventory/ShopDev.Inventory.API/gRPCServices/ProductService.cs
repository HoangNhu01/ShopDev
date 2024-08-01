using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.API.Protos;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Infrastructure.Persistence;

namespace ShopDev.Inventory.API.gRPCServices
{
    public class ProductService : ProductProto.ProductProtoBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly InventoryDbContext _dbContext;

        public ProductService(ILogger<ProductService> logger, InventoryDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override async Task<ProductResponse> FindById(
            ProductRequest request,
            ServerCallContext context
        )
        {
            var product =
                await _dbContext
                    .Set<Domain.Products.Product>()
                    .AsNoTracking()
                    .Include(x => x.Spus.Where(x => x.Id == request.SpuId))
                    .FirstOrDefaultAsync(x => x.Id == request.Id)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            // Nếu số lượng trong kho bé hơn
            if (product.Spus.Count > 0 && product.Spus[0].Stock < request.Quantity)
            {
                throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            }
            if (product.Spus.Count == 0)
            {
                throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            }
            if (product.Variations.Count != product.Spus[0].Index.Count)
            {
                throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            }
            ProductResponse productResponse =
                new()
                {
                    Product = new()
                    {
                        Id = product.Id,
                        Price = product.Price,
                        Name = product.Name,
                        ShopId = product.ShopId,
                        ThumbUri = product.ThumbUri,
                        Title = product.Title,
                        Quantity = product.Spus[0].Stock,
                        SpuId = product.Spus[0].Id,
                    }
                };
            productResponse.Product.Spus.AddRange(
                product
                    .Spus[0]
                    .Index.Select(
                        (x, index) =>
                        {
                            string opt = product.Variations[index].Options[x];
                            return new Protos.Spu
                            {
                                Options = opt,
                                Name = product.Variations[index].Name,
                            };
                        }
                    )
            );
            return productResponse;
        }
    }
}
