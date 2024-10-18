using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.Database;
using ShopDev.Order.Domain.Order;

namespace ShopDev.Order.Domain.Shipment
{
    [Table(nameof(OrderProcess), Schema = DbSchemas.SDOrder)]
    [Index(
        nameof(WarehouseName),
        nameof(ProcessStatus),
        AllDescending = true,
        Name = $"IX_{nameof(OrderProcess)}",
        IsUnique = false
    )]
    public class OrderProcess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid OrderId { set; get; }
        public virtual OrderGen Order { get; } = null!;

        /// <summary>
        /// Kho hàng
        /// </summary>
        public required string WarehouseName { get; set; }

        /// <summary>
        /// Trạng thái tiến trình
        /// </summary>
        public int ProcessStatus { get; set; }

        /// <summary>
        /// Thời gian
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}
