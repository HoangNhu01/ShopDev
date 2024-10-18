namespace CR.PVCB.Configs
{
    public class PvcbEkycConfig
    {
        /// <summary>
        /// Path lấy token
        /// </summary>
        public const string GetTokenPath = "/auth/realms/pvcombank/protocol/openid-connect/token";

        /// <summary>
        /// Path lấy thông tin
        /// </summary>
        public const string GetInformationPath =
            "/auth/realms/pvcombank/protocol/openid-connect/token";
        public required string BaseUrl { get; set; }
        public required string TokenUrl { get; set; }
    }

    public class PvcbEkycSecrets
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string PublicKey { get; set; }
        public required PvcbEkycDecrypt EkycDecrypt { get; set; }
    }

    public class PvcbEkycDecrypt
    {
        public required string DecryptKey { get; set; }
        public required string DecryptIv { get; set; }
    }
}
