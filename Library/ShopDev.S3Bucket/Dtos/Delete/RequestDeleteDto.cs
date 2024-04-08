using System.Text.Json.Serialization;

namespace ShopDev.S3Bucket.Dtos.Delete
{
    /// <summary>
    /// Request xóa file
    /// </summary>
    public class RequestDeleteDto
    {
        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("folderType")]
        public string? FolderType { get; set; }
    }
}
