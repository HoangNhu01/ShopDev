using Microsoft.EntityFrameworkCore;

namespace ShopDev.Order.ApplicationServices.CartModule.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        //[BsonElement("name")]
        public required string Name { get; set; }

        //[BsonElement("title")]
        public required string Title { get; set; }

        //[BsonElement("shopId")]
        public int Quantity { set; get; }
        public double Price { set; get; }
        public int ShopId { get; set; }
        public int SpuId { get; set; }

        [Unicode(false)]
        public required string ThumbUri { get; set; }

        //[BsonElement("stock")]
        public List<SpuDto> Spus { get; set; } = [];
    }

    public class SpuDto
    {
        //[BsonElement("variation_name")]
        public required string Name { get; set; }

        //[BsonElement("variation_options")]
        public required string Options { get; set; }
    }
}
