using ShopDev.ApplicationBase.Common;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Abstract
{
    public interface IProductService
    {
        void Create(ProductCreateDto input);
        ProductDetailDto FindById(int id);
        void Update(ProductUpdateDto input);
        PagingResult<ProductDetailDto> FindAll(ProductFilterDto input);

    }
}
