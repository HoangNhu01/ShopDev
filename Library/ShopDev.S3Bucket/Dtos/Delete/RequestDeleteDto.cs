using System.Text.Json.Serialization;

namespace CR.S3Bucket.Dtos.Delete
{
    /// <summary>
    /// Request xóa file
    /// </summary>
    public class RequestDeleteDto
    {
        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }
    }
}
