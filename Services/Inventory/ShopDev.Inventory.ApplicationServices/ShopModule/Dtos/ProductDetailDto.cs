namespace ShopDev.Inventory.ApplicationServices.ShopModule.Dtos
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
    }
}
