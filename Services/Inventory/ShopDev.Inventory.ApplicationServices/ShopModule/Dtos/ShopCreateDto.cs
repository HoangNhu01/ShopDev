using System.ComponentModel.DataAnnotations;
using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Inventory.ApplicationServices.ShopModule.Dtos
{
    public class ShopCreateDto
    {
        public required string Name { get; set; }

        [CustomMaxLength(2500)]
        public required string Description { get; set; }

        [CustomMaxLength(255)]
        public required string Title { get; set; }
        public required string ThumbUri { get; set; }
    }
}
