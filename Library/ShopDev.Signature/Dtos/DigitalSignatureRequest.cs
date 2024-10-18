using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CR.Signature.Dtos
{
    public class MetaDataSigning
    {
        [JsonPropertyName("visibleSignature")]
        public bool VisibleSignature { get; set; }

        [JsonPropertyName("imageandText")]
        public bool ImageandText { get; set; }

        [JsonPropertyName("textColor")]
        public required string TextColor { get; set; }

        [JsonPropertyName("location")]
        public required string Location { get; set; }

        [JsonPropertyName("signReason")]
        public required string SignReason { get; set; }

        [JsonPropertyName("pageno")]
        public int Pageno { get; set; }

        [JsonPropertyName("signatureBox")]
        public required string SignatureBox { get; set; }

        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; }

        [JsonPropertyName("signatureImage")]
        public required string SignatureImage { get; set; }
    }

    public class DigitalSignatureRequest
    {
        [JsonPropertyName("requestID")]
        public required string RequestID { get; set; }

        [JsonPropertyName("signingFileData")]
        public required string SigningFileData { get; set; }

        [JsonPropertyName("mimeType")]
        public required string MimeType { get; set; }

        [JsonPropertyName("metaDataSigning")]
        public MetaDataSigning MetaDataSigning { get; set; } = null!;
    }
}
