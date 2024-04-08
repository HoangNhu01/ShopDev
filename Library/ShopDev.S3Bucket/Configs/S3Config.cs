namespace ShopDev.S3Bucket.Configs
{
    public class S3Config
    {
        public const string XClientSourceHeader = "X-Client-Source";
        public const string XClientSourceValue = "meeybank";

        public const string MediaPath = "/v1/media";
        public const string MovePath = "/v1/media/move";

        public string BaseUrl { get; set; } = null!;
        public string ViewMediaUrl { get; set; } = null!;
    }
}
