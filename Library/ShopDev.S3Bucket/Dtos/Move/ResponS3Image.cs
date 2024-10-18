using System.Text.Json.Serialization;
using CR.S3Bucket.Dtos.Media;

namespace CR.S3Bucket.Dtos.Move
{
    public class ResponseS3Image : S3Image
    {
        [JsonPropertyName("s3KeyOld")]
        public string? S3KeyOld { get; set; }

        [JsonPropertyName("expiredAt")]
        public int ExpiredAt { get; set; }

        [JsonPropertyName("isExists")]
        public bool IsExists { get; set; }
    }
}
