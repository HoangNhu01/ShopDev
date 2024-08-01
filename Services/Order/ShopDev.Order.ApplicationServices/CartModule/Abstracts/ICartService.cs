using ShopDev.Order.ApplicationServices.CartModule.Dtos;

namespace ShopDev.Order.ApplicationServices.CartModule.Abstract
{
    public interface ICartService
    {
        Task AddToCart(CartUpdateDto input);
        Task<List<CartItemDto>> ViewCart();
        Task RemoveFromCart(CartUpdateDto input);
    }
}
