using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.TokenDtos
{
    /// <summary>
    /// Response get token từ Pvcb
    /// </summary>
    public class PvcbGetTokenReponseDto
    {
        /// <summary>
        /// access_token, sử dụng cho mỗi request từ pvcb
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? access_token { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int expires_in { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public int? refresh_expires_in { get; set; }

        /// <summary>
        /// Loại token
        /// </summary>
        [JsonPropertyName("token_type")]
        public string? token_type { get; set; }

        [JsonPropertyName("not-before-policy")]
        public int? notbeforepolicy { get; set; }

        [JsonPropertyName("scope")]
        public string? scope { get; set; }
    }
}
