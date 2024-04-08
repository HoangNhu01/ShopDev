using System.Text.Json.Serialization;

namespace ShopDev.S3Bucket.Dtos.Move
{
    public class RequestMoveDto
    {
        [JsonPropertyName("images")]
        public List<ImageMoveDto>? Images { get; set; }

        [JsonPropertyName("videos")]
        public List<VideoMoveDto>? Videos { get; set; }

        [JsonPropertyName("files")]
        public List<FileMoveDto>? Files { get; set; }
    }

    public class FileMoveDto
    {
        [JsonPropertyName("s3Key")]
        public string S3Key { get; set; } = null!;
    }

    public class ImageMoveDto
    {
        [JsonPropertyName("s3Key")]
        public string S3Key { get; set; } = null!;
    }

    public class VideoMoveDto
    {
        [JsonPropertyName("s3Key")]
        public string S3Key { get; set; } = null!;
    }
}
