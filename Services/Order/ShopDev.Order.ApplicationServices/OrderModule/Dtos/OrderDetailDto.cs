namespace ShopDev.Order.ApplicationServices.OrderModule.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { set; get; }

        public int UserId { set; get; }
        public required string ShipName { set; get; }
        public required string ShipAddress { set; get; }
        public required string ShipEmail { set; get; }
        public required string ShipPhoneNumber { set; get; }
        public double TotalPrice { set; get; }

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public int Status { set; get; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public int PaymentStatus { set; get; }
        public List<OrderDetailDto> OrderDetails { get; set; } = [];
    }

    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int ProductId { set; get; }
        public ProductDto Product { get; set; } = null!;
        public int StockStatus { get; set; }
    }
}
