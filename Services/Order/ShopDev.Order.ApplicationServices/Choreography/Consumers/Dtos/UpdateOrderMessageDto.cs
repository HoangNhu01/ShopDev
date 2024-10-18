using Microsoft.EntityFrameworkCore;

namespace ShopDev.Order.ApplicationServices.Choreography.Consumers.Dtos
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
        public int EventType { get; set; }
        public required string Message { get; set; }
    }
}
