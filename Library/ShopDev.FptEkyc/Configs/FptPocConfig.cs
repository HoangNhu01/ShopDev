namespace CR.FptEkyc.Configs
{
    public class FptPocConfig
    {
        public required string BaseUrl { get; set; }
        public required string Code { get; set; }
        public required string Token { get; set; }

        public const string ApiOcrId = "api/public/all/doc-noi-dung-ocr";
        public const string ApiLiveness = "api/public/all/xac-thuc-khuon-mat";
        public const string ApiFaceMatch = "api/public/all/xac-thuc-khach-hang";
        public const string ApiCodeTransaction = "api/public/all/ma-giao-dich";
        public const string CodeTransaction = "code_transaction";
    }
}
