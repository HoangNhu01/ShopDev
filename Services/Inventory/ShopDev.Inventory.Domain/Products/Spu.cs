using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;

namespace ShopDev.Inventory.Domain.Products
{
    [Table(nameof(Spu), Schema = DbSchemas.SDInventory)]
    public class Spu : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[BsonElement("productId")]
        public int ProductId { get; set; }

        //[NotMapped]
        public virtual Product Product { get; } = null!;

        //[BsonElement("index")]
        public List<int> Index { get; set; } = [];

        //[BsonElement("price")]
        public double Price { get; set; }

        //[BsonElement("stock")]
        public int Stock { get; set; }

        public byte[] Version { get; set; } = null!;

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
