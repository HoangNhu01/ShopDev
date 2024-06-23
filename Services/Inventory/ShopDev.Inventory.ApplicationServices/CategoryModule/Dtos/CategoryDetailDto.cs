namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos
{
    public class CategoryDetailDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { set; get; }
        public int Status { set; get; }
        public string? ImageUri { set; get; }
        public string? S3key { set; get; }
        public DateTime CreateDate { get; set; }
    }
}
