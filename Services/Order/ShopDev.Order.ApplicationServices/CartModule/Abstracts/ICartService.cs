using ShopDev.Order.ApplicationServices.CartModule.Dtos;

namespace ShopDev.Order.ApplicationServices.CartModule.Abstract
{
    public interface ICartService
    {
        Task AddToCart(CartUpdateDto input);
        Task<List<ProductDto>> ViewCart();
        Task RemoveFromCart(CartUpdateDto input);
    }
}
