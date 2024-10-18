using CR.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.Signature.Localization
{
    public class SignatureLocalization : LocalizationBase, ISignatureLocalization
    {
        public SignatureLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("CR.Signature.Localization.SourceFiles");
        }
    }
}
