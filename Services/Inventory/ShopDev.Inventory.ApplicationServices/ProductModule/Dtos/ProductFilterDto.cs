using Microsoft.AspNetCore.Mvc;
using ShopDev.ApplicationBase.Common;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Dtos
{
    public class ProductFilterDto : PagingRequestBaseDto
    {
        [FromQuery(Name = "isFeatured")]
        public bool? IsFeatured { get; set; }

        [FromQuery(Name = "price")]
        public double? Price { get; set; }

        [FromQuery(Name = "shopName")]
        public string? ShopName { get; set; }

        [FromQuery(Name = "productName")]
        public string? ProductName { get; set; }

        [FromQuery(Name = "shopId")]
        public int? ShopId { get; set; }

        [FromQuery(Name = "attributeNames")]
        public List<string>? AttributeNames { get; set; }

        [FromQuery(Name = "categoryNames")]
        public List<string>? CategoryNames { get; set; }
    }
}
