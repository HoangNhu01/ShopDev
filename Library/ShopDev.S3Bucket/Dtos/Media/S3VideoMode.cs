using System.Text.Json.Serialization;

namespace ShopDev.S3Bucket.Dtos.Media
{
    public class S3VideoMode
    {
        [JsonPropertyName("360p")]
        public Mode360P? Mode360p { get; set; }

        [JsonPropertyName("480p")]
        public Mode480P? Mode480p { get; set; }

        [JsonPropertyName("720p")]
        public Mode720P? Mode720p { get; set; }

        [JsonPropertyName("1080p")]
        public Mode1080P? Mode1080p { get; set; }
    }

    public class Mode1080P
    {
        [JsonPropertyName("urlVod")]
        public string? UrlVod { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("isConverted")]
        public bool IsConverted { get; set; }
    }

    public class Mode360P
    {
        [JsonPropertyName("urlVod")]
        public string? UrlVod { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("isConverted")]
        public bool IsConverted { get; set; }
    }

    public class Mode480P
    {
        [JsonPropertyName("urlVod")]
        public string? UrlVod { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("isConverted")]
        public bool IsConverted { get; set; }
    }

    public class Mode720P
    {
        [JsonPropertyName("urlVod")]
        public string? UrlVod { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("s3Key")]
        public string? S3Key { get; set; }

        [JsonPropertyName("isConverted")]
        public bool IsConverted { get; set; }
    }
}
