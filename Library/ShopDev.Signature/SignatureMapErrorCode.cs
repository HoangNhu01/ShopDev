using CR.ApplicationBase.Localization;
using CR.Signature.Constants;
using CR.Signature.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.Signature
{
    public class SignatureMapErrorCode
        : MapErrorCodeBase<SignatureErrorCode>,
            ISignatureMapErrorCode
    {
        protected override string PrefixError => "error_signature_";

        public SignatureMapErrorCode(
            ISignatureLocalization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
