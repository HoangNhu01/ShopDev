namespace ShopDev.Inventory.ApplicationServices.Choreography.Consumers.Dtos
{
    /// <summary>
    /// Message đẩy vào queue sang inventory cập nhật kho hàng
    /// </summary>
    public class UpdateStockMessageDto
    {
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id sản chi tiết sản phẩm theo loại
        /// </summary>
        public int SpuId { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
    }
}
