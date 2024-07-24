using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Order.Domain.Products;

namespace ShopDev.Order.Domain.Order
{
    [Table(nameof(OrderDetail), Schema = DbSchemas.SDOrder)]
    [Index(
        nameof(ProductId),
        nameof(SpuId),
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(OrderDetail)}",
        IsUnique = false
    )]
    public class OrderDetail : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid OrderId { set; get; }
        public int ProductId { set; get; }
        public int SpuId { set; get; }
        public virtual OrderGen Order { get; } = null!;
        public Product Product { get; set; } = null!;
        public int StockStatus { get; set; }
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
