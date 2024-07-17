using System.Text.Json.Serialization;

namespace ShopDev.InfrastructureBase.Files.Dtos
{
    public class ResponseUploadDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }
        [JsonPropertyName("olds3Key")]
        public string? OldS3Key { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }
    }
}
