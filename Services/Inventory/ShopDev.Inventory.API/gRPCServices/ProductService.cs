using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ShopDev.ApplicationBase;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.API.Protos;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
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
                    .Set<Product>()
                    .AsNoTracking()
                    .Include(x => x.Spus.Where(x => x.Id == request.SpuId))
                    .FirstOrDefaultAsync(x => x.Id == request.Id)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            ProductResponse productResponse =
                new()
                {
                    Id = product.Id,
                    Description = product.Description,
                    Price = product.Price,
                    Name = product.Name,
                    ShopId = product.ShopId,
                    ThumbUri = product.ThumbUri,
                    Title = product.Title
                };
            foreach (var item in product.Spus)
            {
                if(item.Index.Count > 0)
                {

                }
            }
            return productResponse;
        }
    }
}
