using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopDev.Abstractions.EntitiesBase.Interfaces;

namespace ShopDev.Inventory.Domain.Categories
{
    public class Category : IFullAudited
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [MaxLength(100)]
        [BsonElement("name")]
        public required string Name { get; set; }

        [MaxLength(1024)]
        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("sortOrder")]
        public int SortOrder { set; get; }

        [BsonElement("isShowOnHome")]
        public bool IsShowOnHome { set; get; }

        [BsonElement("parentId")]
        public int? ParentId { set; get; }
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
