using MongoDB.Bson;
using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Dtos
{
	public class ProductUpdateDto
	{
		public string Id { get; set; }

		[CustomMaxLength(100)]
		public required string Name { get; set; }

		[CustomMaxLength(2500)]
		public required string Description { get; set; }

		[CustomMaxLength(255)]
		public required string Title { get; set; }
		public required string ThumbUri { get; set; }
		public double Price { get; set; }
		public List<AttributeUpdateDto> Attributes { get; set; } = [];
		public List<VariationUpdateDto> Variations { get; set; } = [];
		public List<SpuUpdateDto> Spus { get; set; } = [];
	}

	public class AttributeUpdateDto
	{
		[CustomMaxLength(255)]
		public required string Name { get; set; }

		[CustomMaxLength(255)]
		public required string Value { get; set; }
	}

	public class VariationUpdateDto
	{
		[CustomMaxLength(255)]
		public required string Name { get; set; }

		[CustomMaxLength(255)]
		public List<string> Options { get; set; } = [];
	}

	public class SpuUpdateDto
	{
		public string? Id { get; set; }
		public List<int> Index { get; set; } = [];
		public double Price { get; set; }
		public int Stock { get; set; }
	}
}
