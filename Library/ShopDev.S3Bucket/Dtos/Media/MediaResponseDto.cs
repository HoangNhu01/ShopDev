using System.Text.Json.Serialization;

namespace CR.S3Bucket.Dtos.Media
{
    public class MediaResponseDto
    {
        [JsonPropertyName("images")]
        public List<S3Image>? Images { get; set; }

        [JsonPropertyName("videos")]
        public List<S3Video>? Videos { get; set; }

        [JsonPropertyName("files")]
        public List<S3File>? Files { get; set; }
    }
}
