using ShopDev.ApplicationBase.Common.Validations;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;

namespace ShopDev.Order.ApplicationServices.OrderModule.Dtos
{
    public class OrderCreateDto
    {
        [CustomRequired]
        public required string ShipName { get; set; }

        [CustomRequired]
        public required string ShipPhoneNumber { get; set; }

        [CustomRequired]
        public required string ShipAddress { get; set; }

        [CustomRequired]
        public required string ShipEmail { get; set; }

        [CustomRequired]
        public int PaymentStatus { get; set; }

        public double TotalPrice { get; set; }
        public List<ProductDto> CartItems { get; set; } = [];
    }
}
