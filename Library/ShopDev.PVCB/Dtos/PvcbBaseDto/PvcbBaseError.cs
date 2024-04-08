using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.PvcbBaseDto
{
    public class PvcbBaseError
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
