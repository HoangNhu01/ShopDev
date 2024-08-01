using Microsoft.EntityFrameworkCore;

namespace ShopDev.Order.ApplicationServices.CartModule.Dtos
{
    public class CartItemDto
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

        public required string ThumbUri { get; set; }

        //[BsonElement("stock")]
        public List<CartDetailDto> Spus { get; set; } = [];
    }

    public class CartDetailDto
    {
        //[BsonElement("variation_name")]
        public required string Name { get; set; }

        //[BsonElement("variation_options")]
        public required string Options { get; set; }
    }
}
