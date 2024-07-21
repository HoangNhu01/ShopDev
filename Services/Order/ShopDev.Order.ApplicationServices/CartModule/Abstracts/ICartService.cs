namespace ShopDev.Order.ApplicationServices.CartModule.Abstract
{
    public interface ICartService
    {
        Task AddToCart(int id, string languageId, int clientQuantity);
    }
}
