using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;

namespace ShopDev.Inventory.ApplicationServices.ShopModule.Dtos
{
    public class ShopDetailDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Title { get; set; }
        public required string ThumbUri { get; set; }
        public int OwnerId { get; set; }
        public required string OwnerName { get; set; }

        [JsonIgnore]
        public string? Products { get; set; }

        [NotMapped]
        public List<ProductDetailDto> ProductDetails { get; set; } = [];
        public DateTime? CreatedDate { get; set; }
    }
}
