using Microsoft.EntityFrameworkCore;

namespace ShopDev.Order.Domain.Products
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        //[BsonElement("name")]
        public required string Name { get; set; }

        [MaxLength(255)]
        //[BsonElement("title")]
        public required string Title { get; set; }

        //[BsonElement("shopId")]
        public int Quantity { set; get; }
        public double Price { set; get; }
        public int ShopId { get; set; }

        [Unicode(false)]
        public required string ThumbUri { get; set; }

        //[BsonElement("stock")]
        public List<Spu> Spus { get; set; } = [];
    }

    public class Spu
    {
        //[BsonElement("variation_name")]
        public required string Name { get; set; }

        //[BsonElement("variation_options")]
        public required string Options { get; set; }
    }
}
