using CR.InfrastructureBase.Exceptions;

namespace CR.Signature.Exceptions
{
    public class SignatureException : BaseException
    {
        public SignatureException(int errorCode)
            : base(errorCode) { }

        public SignatureException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
