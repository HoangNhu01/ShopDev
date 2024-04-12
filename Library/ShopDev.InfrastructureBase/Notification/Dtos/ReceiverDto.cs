using System.Text.Json.Serialization;

namespace ShopDev.InfrastructureBase.Notification.Dtos
{
    public class ReceiverDto
    {
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public EmailNotificationDto? Email { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; }

        [JsonPropertyName("fcm_tokens")]
        public List<string>? FcmTokens { get; set; }

        [JsonPropertyName("apns")]
        public List<string>? APNs { get; set; }
    }

    public class EmailNotificationDto
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}
