using ShopDev.Inventory.ApplicationServices.ShopModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.ShopModule.Abstracts
{
    public interface IShopService
    {
        Task<ShopDetailDto> FindById(int id);
        void Create(ShopCreateDto input);
    }
}
