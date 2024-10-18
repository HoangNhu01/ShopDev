namespace ShopDev.InfrastructureBase.Files.Dtos
{
    public class DownloadFileDto
    {
        public Stream Stream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
