using System.Text.Json.Serialization;

namespace ShopDev.S3Bucket.Dtos.Move
{
    /// <summary>
    /// Respone trả về của api move
    /// </summary>
    public class ResponseMoveDto
    {
        [JsonPropertyName("images")]
        public List<ResponseS3Image>? Images { get; set; }

        [JsonPropertyName("videos")]
        public List<ResponseS3Video>? Videos { get; set; }

        [JsonPropertyName("files")]
        public List<ResponseS3File>? Files { get; set; }
    }
}
