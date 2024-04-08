using System.Text.Json.Serialization;
using ShopDev.S3Bucket.Dtos.Media;

namespace ShopDev.S3Bucket.Dtos.Move
{
    public class ResponseS3File : S3File
    {
        [JsonPropertyName("s3KeyOld")]
        public string? S3KeyOld { get; set; }

        [JsonPropertyName("expiredAt")]
        public int ExpiredAt { get; set; }

        [JsonPropertyName("isExists")]
        public bool IsExists { get; set; }
    }
}
