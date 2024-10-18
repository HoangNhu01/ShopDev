using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Inventory.Domain.Categories
{
    [Table(nameof(CategoryType), Schema = DbSchemas.SDInventory)]
    [Index(
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(CategoryType)}",
        IsUnique = false
    )]
    public class CategoryType : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public virtual Category Category { get; } = null!;
        public virtual Product Product { get; } = null!;
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
