using ShopDev.EntitiesBase.Base;

namespace ShopDev.ConvertFile.Constants
{
    public class ConvertFileErrorCodes : IErrorCode
    {
        public const int InvalidFormatFileExtension = 12000;
        public const int ConvertFileServiceOutOfDuration = 12001;
        public const int InternalServerError = 12002;
    }
}
