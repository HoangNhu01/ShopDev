using ShopDev.InfrastructureBase.Notification.Dtos;
using System.Text.Json.Serialization;

namespace ShopDev.InfrastructureBase.Notification.Dtos
{
    public class SendNotificationDto<T>
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("receiver")]
        public ReceiverDto? Receiver { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("attachments")]
        public List<string>? Attachments { get; set; }
    }
}
