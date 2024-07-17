using System.Text.Json.Serialization;
using CR.S3Bucket.Dtos.Media;

namespace CR.S3Bucket.Dtos.Move
{
    public class ResponseS3Video : S3Video
    {
        [JsonPropertyName("s3KeyOld")]
        public string? S3KeyOld { get; set; }

        [JsonPropertyName("expiredAt")]
        public int ExpiredAt { get; set; }

        [JsonPropertyName("isExists")]
        public bool IsExists { get; set; }
    }
}
