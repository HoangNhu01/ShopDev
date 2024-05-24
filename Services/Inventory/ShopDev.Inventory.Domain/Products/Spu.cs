using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopDev.Inventory.Domain.Products
{
    public class Spu
    {
        [BsonId]
		public string? Id { get; set; }

        [BsonElement("productId")]
		public string? ProductId { get; set; }
        [NotMapped]
        public Product Product { get; } = null!;

        [BsonElement("index")]
        public List<int> Index { get; set; } = [];

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }
    }
}
