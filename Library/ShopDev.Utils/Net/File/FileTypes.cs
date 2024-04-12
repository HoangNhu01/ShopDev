namespace ShopDev.Utils.Net.File
{
    /// <summary>
    /// Extension file hình ảnh dự án
    /// </summary>
    public static class FileTypes
    {
        //Ảnh
        public const string JPG = ".jpg";
        public const string PNG = ".png";
        public const string JPEG = ".jpeg";
        public const string TIFF = ".tiff";
        public const string PDF = ".pdf";
        public const string AI = ".ai";
        public const string SVG = ".svg";
        public const string WEBP = ".webp";
        public const string JFIF = ".jfif";
        public const string GIF = ".gif";

        //File doc
        public const string DOCX = ".docx";

        //File Excel
        public const string XLSX = ".xlsx";

        //Video
        public const string MP4 = ".mp4";
        public const string MOV = ".mov";

        public static readonly string[] CommonImageExtensions = new string[]
        {
            JPG,
            JPEG,
            PNG,
            WEBP
        };

        public static readonly string[] ImageExtensions = (new string[] { SVG })
            .Concat(CommonImageExtensions)
            .ToArray();

        public static readonly string[] JpegExtensions = new string[] { JPG, JPEG };

        public static readonly string[] VideoExtenstions = new string[] { MP4, MOV };
    }
}
