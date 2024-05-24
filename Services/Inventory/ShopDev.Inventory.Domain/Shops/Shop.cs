using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Inventory.Domain.Shops
{
	public class Shop
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
		public double Price { get; set; }
		public required byte[] Version { get; set; }
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
