using CR.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.FptEkyc.Localization
{
    public class FptEkycLocalization : LocalizationBase, IFptEkycLocalization
    {
        public FptEkycLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("CR.FptEkyc.Localization.SourceFiles");
        }
    }
}
