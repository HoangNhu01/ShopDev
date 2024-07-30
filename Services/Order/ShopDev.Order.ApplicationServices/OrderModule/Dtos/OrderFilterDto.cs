using Microsoft.AspNetCore.Mvc;
using ShopDev.ApplicationBase.Common;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Dtos
{
    public class OrderFilterDto : PagingRequestBaseDto
    {
        [FromQuery(Name = "shipName")]
        public string? ShipName { get; set; }

        [FromQuery(Name = "shipAddress")]
        public string? ShipAddress { get; set; }

        [FromQuery(Name = "shipEmail")]
        public string? ShipEmail { get; set; }

        [FromQuery(Name = "shipPhoneNumber")]
        public string? ShipPhoneNumber { get; set; }

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        [FromQuery(Name = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        [FromQuery(Name = "paymentStatus")]
        public int? PaymentStatus { get; set; }
    }
}
