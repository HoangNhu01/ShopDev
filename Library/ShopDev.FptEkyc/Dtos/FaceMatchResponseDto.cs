using System.Text.Json.Serialization;

namespace CR.FptEkyc.Dtos
{
    public class FaceMatchResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("data")]
        public FaceMatchData? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class FaceMatchData
    {
        [JsonPropertyName("isMatch")]
        public bool IsMatch { get; set; }

        [JsonPropertyName("similarity")]
        public double Similarity { get; set; }

        [JsonPropertyName("isBothImgIDCard")]
        public bool IsBothImgIDCard { get; set; }
    }
}
