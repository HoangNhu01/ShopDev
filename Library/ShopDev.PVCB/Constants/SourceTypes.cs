namespace CR.PVCB.Constants
{
    /// <summary>
    /// Nguồn tài khoản truyền vào SourceType trên request, Truyền PAN: nếu là số thẻ / Hoặc ACC: nếu là số tài khoản
    /// </summary>
    public static class SourceTypes
    {
        /// <summary>
        /// Nếu là số thẻ
        /// </summary>
        public const string PAN = "PAN";

        /// <summary>
        /// Nếu là số tài khoản
        /// </summary>
        public const string ACC = "ACC";
    }
}
