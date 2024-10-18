namespace CR.FptEkyc.Configs
{
    /// <summary>
    /// Config cho FPT EKYC gồm api path, key
    /// </summary>
    public class FptEkycConfig
    {
        public const string ApiBaseAddress = "https://api.fpt.ai";
        public const string ApiOCRId = "vision/idr/vnm";
        public const string ApiOCRPassport = "vision/passport/vnm";
        public const string ApiFaceMatch = "dmp/checkface/v1";

        /// <summary>
        /// Api key
        /// </summary>
        public string ApiKey { get; set; } = null!;

        /// <summary>
        /// Giá trị ngưỡng khớp mặt
        /// </summary>
        public double FaceSimilarity { get; set; }

        /// <summary>
        /// Năm hết hạn nếu giá trị ocr trả về rống với cmnd
        /// </summary>
        public int CmndExpiredAddYearIfNull { get; set; }

        /// <summary>
        /// Năm hết hạn nếu giá trị ocr trả về rống với cccd
        /// </summary>
        public int CccdExpiredAddYearIfNull { get; set; }
    }
}
