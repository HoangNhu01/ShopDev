using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Inventory.Domain.Shops
{
    [Table(nameof(Shop), Schema = DbSchemas.SDInventory)]
    [Index(
        nameof(Name),
        nameof(Title),
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(Shop)}",
        IsUnique = false
    )]
    public class Shop : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(2500)]
        public required string Description { get; set; }

        [MaxLength(255)]
        public required string Title { get; set; }
        public required string ThumbUri { get; set; }
        public int OwnerId { get; set; }
        public virtual List<Product> Products { get; } = [];
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
