using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Implements
{
    public class CategoryService : InventoryServiceBase, ICategoryService
    {
        public CategoryService(ILogger<CategoryService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public void Create(CategoryCreateDto input) 
        {
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

        

    }
}
