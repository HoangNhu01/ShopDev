namespace ShopDev.S3Bucket.Configs
{
    public class S3Config
    {
        /// <summary>
        /// Tên bucket
        /// </summary>
        public const string BucketName = "sd-bucket";
        /// <summary>
        /// Url service mino
        /// </summary>
        public string ServiceUrl { get; set; } = null!;
        /// <summary>
        /// Url view file
        /// </summary>
        public string ViewMediaUrl { get; set; } = null!;
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string Username { get; set; } = null!;
        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}
