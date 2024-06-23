using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts
{
    public interface ICategoryService
    {
        void Create(CategoryCreateDto request);
        CategoryDetailDto FindById(int id);
    }
}
