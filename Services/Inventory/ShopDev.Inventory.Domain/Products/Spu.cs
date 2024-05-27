using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopDev.Abstractions.EntitiesBase.Interfaces;

namespace ShopDev.Inventory.Domain.Products
{
    public class Spu : IFullAudited
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

		#region audit
		public DateTime? CreatedDate { get; set; }
		public int? CreatedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public int? ModifiedBy { get; set; }
		public DateTime? DeletedDate { get; set; }
		public int? DeletedBy { get; set; }
		public bool Deleted { get; set; }
		#endregion

	}
}
