using CR.InfrastructureBase.Exceptions;

namespace CR.PVCB.Exceptions
{
    public class PvcbException : BaseException
    {
        public PvcbException(int errorCode)
            : base(errorCode) { }

        public PvcbException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
