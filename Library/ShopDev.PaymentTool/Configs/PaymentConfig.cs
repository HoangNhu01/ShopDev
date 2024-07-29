namespace ShopDev.PaymentTool.Configs
{
    public class PaymentConfig
    {
        public required string Url { get; set; }
        public required string ReturnUrl { get; set; }
        public required string TmnCode { get; set; }
        public required string HashSecret { get; set; }
    }
}
