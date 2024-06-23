using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;

namespace ShopDev.Inventory.Domain.Categories
{
    [Table(nameof(Category), Schema = DbSchemas.SDInventory)]
    [Index(
        nameof(Name),
        nameof(ParentId),
        nameof(IsShowOnHome),
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(Category)}",
        IsUnique = false
    )]
    public class Category : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        //[BsonElement("name")]
        public required string Name { get; set; }

        [MaxLength(1024)]
        //[BsonElement("description")]
        public required string Description { get; set; }

        //[BsonElement("sortOrder")]
        public int SortOrder { set; get; }

        //[BsonElement("isShowOnHome")]
        public bool IsShowOnHome { set; get; }

        //[BsonElement("parentId")]
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
