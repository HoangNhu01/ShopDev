namespace ShopDev.Inventory.ApplicationServices.Choreography.Producers.Dtos
{
    /// <summary>
    /// Message đẩy vào queue sang inventory cập nhật kho hàng
    /// </summary>
    public class UpdateOrderMessageDto
    {
        /// <summary>
        /// Id đơn hàng
        /// </summary>
        public Guid OrderId { get; set; }
        public int MessageType { get; set; }
        public required string Message { get; set; }
    }
}
