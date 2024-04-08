using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.ConvertFile.Exceptions
{
    public class ConvertFileException : BaseException
    {
        public ConvertFileException(int errorCode)
            : base(errorCode) { }

        public ConvertFileException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
