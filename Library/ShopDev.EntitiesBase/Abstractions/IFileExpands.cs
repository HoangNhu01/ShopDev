namespace ShopDev.Abstractions.EntitiesBase.Interfaces
{
    public interface IFileExpands
    {
        public string? FileUrl { get; set; }
        public string? S3Key { get; set; }
    }
}
