namespace ShopDev.S3Bucket.Dtos.Media
{
    public class S3StreamFile
    {
        public required Stream StreamFiles { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
    }
}
