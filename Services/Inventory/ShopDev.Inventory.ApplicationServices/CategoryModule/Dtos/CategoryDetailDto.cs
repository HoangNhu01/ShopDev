namespace ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos
{
    public class CategoryDetailDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int? ParentId { get; set; }
        public bool IsShowOnHome { get; set; }
        public int SortOrder { set; get; }
        public string? ImageUri { set; get; }
        public string? S3key { set; get; }
        public DateTime CreatedDate { get; set; }
    }
}
