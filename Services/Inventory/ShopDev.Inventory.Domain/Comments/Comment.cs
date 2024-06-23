using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Inventory.Domain.Comments
{
    public class Comment
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("star")]
        public int Star { get; set; }

        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("content")]
        public required string Content { get; set; }

        [BsonElement("parentId")]
        public ObjectId ParentId { get; set; }

        [BsonElement("productId")]
        public ObjectId ProductId { get; set; }

        [NotMapped]
        public Product Product { get; } = null!;
        public List<MediaComment> MediaComments { get; } = [];
    }

    public class MediaComment
    {
        [BsonElement("uri")]
        public required string Uri { get; set; }

        [BsonElement("s3Key")]
        public required string S3Key { get; set; }
    }
}
