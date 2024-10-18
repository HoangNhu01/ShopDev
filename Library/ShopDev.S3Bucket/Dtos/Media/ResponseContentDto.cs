using Newtonsoft.Json;

namespace ShopDev.S3Bucket.Dtos.Media
{
    public class ResponseContentDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
