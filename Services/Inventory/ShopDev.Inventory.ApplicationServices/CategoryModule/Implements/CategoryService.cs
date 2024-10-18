using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Inventory.ApplicationServices.Common;
using System.Text.Json;

namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Implements
{
    public class CategoryService : InventoryServiceBase, ICategoryService
    {
        public CategoryService(ILogger<CategoryService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public void Create(CategoryCreateDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");

            _dbContext.Categories.Add(
                new()
                {
                    Description = input.Description,
                    Name = input.Name,
                    IsShowOnHome = input.IsShowOnHome,
                    ParentId = input.ParentId,
                    SortOrder = input.SortOrder,
                }
            );
            _dbContext.SaveChanges();
        }

        public CategoryDetailDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            return _mapper.Map<CategoryDetailDto>(
                _dbContext.Categories.Find(id) ?? throw new UserFriendlyException(10000)
            );
        }
    }
}
