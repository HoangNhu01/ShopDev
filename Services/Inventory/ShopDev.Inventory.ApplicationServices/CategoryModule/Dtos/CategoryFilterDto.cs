using Microsoft.AspNetCore.Mvc;
using ShopDev.ApplicationBase.Common;

namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos
{
    public class CategoryFilterDto : PagingRequestBaseDto
    {
        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [FromQuery(Name = "isShowOnHome")]
        public bool? IsShowOnHome { set; get; }
    }
}
