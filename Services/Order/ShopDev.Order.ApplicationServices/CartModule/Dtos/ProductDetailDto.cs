using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Order.ApplicationServices.CartModule.Dtos
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Title { get; set; }
        public required string ThumbUri { get; set; }
        public double Price { get; set; }
        public int ShopId { get; set; }
        public List<CategoryTypeDetailDto> Categories { get; set; } = [];
        public List<AttributeDetailDto> Attributes { get; set; } = [];
        public List<VariationDetailDto> Variations { get; set; } = [];
        public List<SpuDetailDto> Spus { get; set; } = [];
    }

    public class CategoryTypeDetailDto
    {
        public int CategoryId { get; set; }
        public required string Name { get; set; }
    }

    public class AttributeDetailDto
    {
        [CustomMaxLength(255)]
        public required string Name { get; set; }

        [CustomMaxLength(255)]
        public required List<string> Value { get; set; } = [];
    }

    public class VariationDetailDto
    {
        [CustomMaxLength(255)]
        public required string Name { get; set; }

        [CustomMaxLength(255)]
        public List<string> Options { get; set; } = [];
    }

    public class SpuDetailDto
    {
        public required string Id { get; set; }
        public List<int> Index { get; set; } = [];
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
