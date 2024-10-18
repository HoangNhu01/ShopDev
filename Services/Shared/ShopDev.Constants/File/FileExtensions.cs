namespace ShopDev.Constants.File
{
    public static class FileExtensions
    {
        public const string JPG = ".jpg";
        public const string JPEG = ".jpeg";
        public const string JFIF = ".jfif";
        public const string PNG = ".png";
        public const string Webp = ".webp";
        public const string Svg = ".svg";
        public const string Mp4 = ".mp4";
        public const string Mov = ".mov";

        public static readonly string[] CommonImageExtensions = new string[]
        {
            JPG,
            JPEG,
            JFIF,
            PNG,
            Webp
        };

        public static readonly string[] ImageExtensions = (new string[] { Svg })
            .Concat(CommonImageExtensions)
            .ToArray();

        public static readonly string[] JpegExtensions = new string[] { JPG, JPEG, JFIF };

        public static readonly string[] VideoExtenstions = new string[] { Mp4, Mov };
    }
}
