namespace CR.PVCB.Configs
{
    public class PvcbConfig
    {
        public const string GrantType = "client_credentials";
        public const string XIdempotencyKey = "X-Idempotency-Key";

        /// <summary>
        /// Path lấy token
        /// </summary>
        public const string GetTokenPath = "/auth/realms/pvcombank/protocol/openid-connect/token";

        /// <summary>
        /// path lấy danh sách ngân hàng
        /// </summary>
        public const string GetListBankPath = "/v1/ibft/info/bank";

        /// <summary>
        /// Path truy vấn số dư tài khoản
        /// </summary>
        public const string GetInfoAccount = "/v1/ibft/info/account";

        /// <summary>
        /// path đăng kí tài khỏan ảo
        /// </summary>
        public const string OpenVirtualAcc = "/v1/virtual-account/open-batch-account";

        /// <summary>
        /// path lấy danh sách ngân hàng
        /// </summary>
        public const string GetDetailVirtualAcc = "/v1/virtual-account/info/";

        /// <summary>
        /// path lấy danh sách thông tin giao dịch
        /// </summary>
        public const string GetListTransactionOfVirtualAcc = "/v1/virtual-account/transaction/";

        /// <summary>
        /// path khóa acc
        /// </summary>
        public const string LockVirtualAcc = "/v1/virtual-account/lock/";

        /// <summary>
        /// path mở khóa acc
        /// </summary>
        public const string UnLockVirtualAcc = "/v1/virtual-account/unlock/";

        /// <summary>
        /// path đóng acc
        /// </summary>
        public const string CloseVirtualAcc = "/v1/virtual-account/close/";

        /// <summary>
        /// Truy vấn tài khoản ngân hàng
        /// </summary>
        public const string InquiryPath = "/v1/ibft/transfer/inquiry";

        /// <summary>
        /// Chuyển tiền nội bộ/liên ngân hàng
        /// </summary>
        public const string DepositPath = "/v1/ibft/transfer/deposit";
        public string BaseUrl { get; set; } = null!;
        public string TokenUrl { get; set; } = null!;
        public string BankAccount { get; set; } = null!;
    }

    public class PvcbSecrets
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public PvcbGoogleAuthenticator GoogleAuthenticator { get; set; } = null!;
    }

    public class PvcbGoogleAuthenticator
    {
        public string Issuer { get; set; } = null!;
        public string AccountTitle { get; set; } = null!;
        public string Secret { get; set; } = null!;
    }
}
