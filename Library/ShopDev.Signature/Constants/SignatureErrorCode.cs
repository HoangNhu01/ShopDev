using CR.EntitiesBase.Base;

namespace CR.Signature.Constants
{
    public class SignatureErrorCode : IErrorCode
    {
        public const int TimeOutToSign = 13000;
        public const int SignatureCanBeSign = 13001;
        public const int InternalServerError = 13002;

        public const string InnerMessageCode = "1001";
        public const int InnerMessageCodeDetail = 13003;
    }
}
