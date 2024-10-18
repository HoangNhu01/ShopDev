using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Order.ApplicationServices.CartModule.Dtos
{
    public class CartUpdateDto
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
