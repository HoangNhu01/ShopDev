using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos
{
    public class CategoryCreateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { set; get; }
        public bool IsShowOnHome { set; get; }
        public string? ImageUri { set; get; }
        public string? S3key { set; get; }
    }
}
