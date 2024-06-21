using ShopDev.Inventory.ApplicationServices.ShopModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.ShopModule.Abstracts
{
    public interface IShopService
    {
        Task<ShopDetailDto> FindByIdAsync(int id);
        void Create(ShopCreateDto input);
    }
}
