using CR.InfrastructureBase.Exceptions;

namespace CR.FptEkyc.Exceptions
{
    public class FptEkycException : BaseException
    {
        public FptEkycException(int errorCode)
            : base(errorCode) { }

        public FptEkycException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
