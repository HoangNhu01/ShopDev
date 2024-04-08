using System.Text.Json.Serialization;

namespace ShopDev.S3Bucket.Dtos.Media
{
    public class S3Video
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("urlVod")]
        public string? UrlVod { get; set; }

        [JsonPropertyName("isConverted")]
        public bool IsConverted { get; set; }

        [JsonPropertyName("mode")]
        public S3VideoMode? Mode { get; set; }

        [JsonPropertyName("screenshots")]
        public List<Screenshot>? Screenshots { get; set; }
    }
}
