using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        // Thuộc tính LazyLoader
        [NotMapped]
        protected ILazyLoader? LazyLoader { get; }
    }
}
