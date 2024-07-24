using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.EntitiesBase.Base;
using ShopDev.Order.Domain.Shipment;

namespace ShopDev.Order.Domain.Order
{
    [Table(nameof(OrderGen), Schema = DbSchemas.SDOrder)]
    [Index(
        nameof(OrderDate),
        nameof(UserId),
        nameof(Deleted),
        nameof(ShipName),
        nameof(PaymentStatus),
        nameof(Status),
        AllDescending = true,
        Name = $"IX_{nameof(OrderGen)}",
        IsUnique = false
    )]
    public class OrderGen : Entity<Guid>, IFullAudited
    {
        public DateTime OrderDate { set; get; }
        public int UserId { set; get; }
        public required string ShipName { set; get; }
        public required string ShipAddress { set; get; }
        public required string ShipEmail { set; get; }
        public required string ShipPhoneNumber { set; get; }
        public double TotalPrice { set; get; }

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public int Status { set; get; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public int PaymentStatus { set; get; }
        public virtual List<OrderDetail> OrderDetails { get; } = [];
        public virtual List<OrderProcess> OrderProcesses { get; } = [];
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
