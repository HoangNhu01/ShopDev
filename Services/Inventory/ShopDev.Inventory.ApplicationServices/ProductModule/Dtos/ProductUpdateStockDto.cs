namespace ShopDev.Inventory.ApplicationServices.ProductModule.Dtos
{
    public class ProductUpdateStockDto
    {
        public int ProductId { get; set; }
        public int SpuId { get; set; }
        public int Quantity { get; set; }
    }
}
