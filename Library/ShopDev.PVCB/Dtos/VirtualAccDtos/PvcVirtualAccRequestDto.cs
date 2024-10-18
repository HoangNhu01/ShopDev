using System.Text.Json.Serialization;
using CR.ApplicationBase.Common.Validations;
using CR.PVCB.Constants;
using Newtonsoft.Json;

namespace CR.PVCB.Dtos.VirtualAccDtos
{
    /// <summary>
    /// Model tạo acc theo lô
    /// </summary>
    public class PvcVirtualAccRequestDto
    {
        /// <summary>
        /// Quy tắc tạo tên tài khoản ảo
        /// </summary>
        [StringRange(
            AllowableValues = new string[]
            {
                RegulationParameters.AccordingCustomerName,
                RegulationParameters.AccordingPaymentAcc,
                RegulationParameters.NameRuleManual
            }
        )]
        [JsonPropertyName("nameRule")]
        public required string NameRule { get; set; }

        /// <summary>
        /// Quy tắc tạo số tài khoản ảo
        /// </summary>
        [StringRange(
            AllowableValues = new string[]
            {
                RegulationParameters.AccordingParameter,
                RegulationParameters.NumberRuleManual,
            }
        )]
        [JsonPropertyName("numberRule")]
        public required string NumberRule { get; set; }

        /// <summary>
        /// Số lượng tài khoản ảo cần tạo
        /// </summary>
        [JsonPropertyName("numofVA")]
        public required string NumofVA { get; set; }

        [JsonPropertyName("createVABatch")]
        public required List<PvcVirtualAccDetailRequestDto> CreateVABatch { get; set; } = new();

        /// <summary>
        /// Mô tả
        /// </summary>
        [JsonPropertyName("description")]
        public required string Description { get; set; }

        /// <summary>
        /// Kênh giao dịch
        /// </summary>
        [JsonPropertyName("channel")]
        public required string Channel { get; set; }
    }

    public class PvcVirtualAccDetailRequestDto
    {
        [JsonPropertyName("vaDetail")]
        public required VaDetail VaDetail { get; set; }
    }

    public class VaDetail
    {
        /// <summary>
        /// Số lượng tài khoản ảo cần tạo
        /// </summary>
        [JsonPropertyName("vaNumber")]
        public required string VaNumber { get; set; }

        /// <summary>
        /// Số lượng tài khoản ảo cần tạo
        /// </summary>
        [JsonPropertyName("vaName")]
        public required string VaName { get; set; }
    }
}
