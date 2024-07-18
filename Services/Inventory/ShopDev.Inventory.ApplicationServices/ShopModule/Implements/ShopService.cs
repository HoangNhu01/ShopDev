using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.ApplicationServices.Common;
using ShopDev.Inventory.ApplicationServices.ShopModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.ShopModule.Dtos;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ShopDev.Inventory.ApplicationServices.ShopModule.Implements
{
    public class ShopService : InventoryServiceBase, IShopService
    {
        private readonly AuthenticationDbContext _authenticationDbContext;
        private static JsonSerializerOptions jsonSerializerOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ShopService(
            ILogger<ShopService> logger,
            IHttpContextAccessor httpContext,
            AuthenticationDbContext authenticationDbContext
        )
            : base(logger, httpContext)
        {
            _authenticationDbContext = authenticationDbContext;
        }

        public void Create(ShopCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            int userId = _httpContext.GetCurrentCustomerId();
            _dbContext.Shops.Add(
                new()
                {
                    Description = input.Description,
                    Name = input.Name,
                    OwnerId = userId,
                    ThumbUri = input.ThumbUri,
                    Title = input.Title,
                }
            );
            _dbContext.SaveChanges();
        }

        public async Task<ShopDetailDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var query =
                $@"
            SELECT 
                a.Id, a.Description, b.FullName as OwnerName, 
                a.Name, a.OwnerId, a.ThumbUri, a.Title, 
                a.CreatedDate, 
                (
                    SELECT * 
                    FROM [InventoryDb].[sd_inventory].[Product] c 
                    WHERE c.ShopId = a.Id 
                    ORDER BY c.Id
                    OFFSET {0} ROWS
                    FETCH NEXT {10} ROWS ONLY
                    FOR JSON PATH
                ) AS Products
            FROM 
                [InventoryDb].[sd_inventory].[Shop] a
            LEFT JOIN 
                [ShopDevDB].[dbo].[User] b 
            ON 
                a.OwnerId = b.Id
            WHERE 
                a.Id = {id}";

            var joinedData =
                await _dbContext
                    .Database.SqlQuery<ShopDetailDto>(FormattableStringFactory.Create(query))
                    .FirstOrDefaultAsync() ?? throw new UserFriendlyException(10000);
            if (!string.IsNullOrEmpty(joinedData.Products))
            {
                joinedData.ProductDetails = JsonSerializer.Deserialize<List<ProductDetailDto>>(
                    joinedData.Products,
                    jsonSerializerOptions
                )!;
            }
            return joinedData;
        }
    }
}
