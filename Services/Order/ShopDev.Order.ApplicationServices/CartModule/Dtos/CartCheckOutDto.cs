using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Order.ApplicationServices.CartModule.Dtos
{
    public class CartCheckOutDto
    {
        [CustomRequired]
        public required string UserName { get; set; }

        [CustomRequired]
        public required string PhoneNumber { get; set; }

        [CustomRequired]
        public required string Address { get; set; }

        [CustomRequired]
        public required string Email { get; set; }

        [CustomRequired]
        public int PaymentStatus { get; set; }

        public double TotalPrice { get; set; }
        public List<CartItemDto> CartItems { get; set; } = [];
    }
}
