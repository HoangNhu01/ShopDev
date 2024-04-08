using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CR.PVCB.Dtos.VirtualAccDtos
{
    public class PvcRegisterVirtualAccReponseData
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("successIndicator")]
        public string? SuccessIndicator { get; set; }

        [JsonPropertyName("application")]
        public string? Application { get; set; }

        [JsonPropertyName("messages")]
        public List<object>? Messages { get; set; }

        [JsonPropertyName("messageId")]
        public MessageId? MessageId { get; set; }
    }

    public class MessageId { }
}
