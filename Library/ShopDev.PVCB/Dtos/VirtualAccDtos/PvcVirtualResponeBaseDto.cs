using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.VirtualAccDtos
{
    public class PvcVirtualResponeBaseDto<T> : PvcVirtualResponeBaseDto
        where T : class
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class PvcVirtualResponeBaseDto
    {
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
