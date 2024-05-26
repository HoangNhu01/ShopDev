using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopDev.Inventory.Domain.Products
{
    public class Spu
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("productId")]
        public ObjectId ProductId { get; set; }

        [NotMapped]
        public virtual Product Product { get; } = null!;

        [BsonElement("index")]
        public List<int> Index { get; set; } = [];

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }

        [BsonElement("version")]
        public int Version { get; set; }
    }
}
