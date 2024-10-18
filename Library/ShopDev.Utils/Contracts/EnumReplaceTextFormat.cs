namespace ShopDev.Utils.Contracts
{
    /// <summary>
    /// Các dạng format cho các biến replace
    /// </summary>
    public enum EnumReplaceTextFormat
    {
        /// <summary>
        /// Format dạng: "wAr aNd pEaCe" to titlecase: War And Peace
        /// </summary>
        TitleCase,

        /// <summary>
        /// Format dạng: M, F sang Nam, Nữ
        /// </summary>
        Gender,

        /// <summary>
        /// Số dạng Việt Nam: ngăn cách phần nghìn bằng dấu . phần thập phân bằng dấu ,
        /// </summary>
        NumberVietNam,

        /// <summary>
        /// Chuyển số thành chữ viết: vd nghìn, tỉ,...
        /// </summary>
        NumberToWord,

        /// <summary>
        /// Format dạng: "wAr aNd pEaCe" to titlecase: WAR AND PEACE
        /// </summary>
        UpperCase,
    }
}
