using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Inventory.ApplicationServices.Common;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Utils.Linq;

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
                _dbContext.Categories.Find(id)
                    ?? throw new UserFriendlyException(InventoryErrorCode.CategoryNotFound)
            );
        }

        public PagingResult<CategoryDetailDto> FindAll(CategoryFilterDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var categoryQuery = GetIQueryableResult<CategoryDetailDto, Category>(
                selector: x =>
                    new()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SortOrder = x.SortOrder,
                        Description = x.Description,
                        IsShowOnHome = x.IsShowOnHome,
                        ParentId = x.ParentId,
                    },
                predicate: x =>
                    (string.IsNullOrEmpty(input.Name) || x.Name.Contains(input.Name))
                    && (!input.IsShowOnHome.HasValue || x.IsShowOnHome == input.IsShowOnHome.Value)
            //include: x => x.Include(x => x.Shop).Include(x => x.Categories).ThenInclude(x => x.Category)
            );
            var result = new PagingResult<CategoryDetailDto>
            {
                // đếm tổng trước khi phân trang
                TotalItems = categoryQuery.Count()
            };
            categoryQuery = categoryQuery.OrderDynamic(input.Sort);
            if (input.PageSize != -1)
            {
                categoryQuery = categoryQuery.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = categoryQuery;
            return result;
        }
    }
}
