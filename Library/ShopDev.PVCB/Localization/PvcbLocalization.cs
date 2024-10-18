using CR.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.PVCB.Localization
{
    public class PvcbLocalization : LocalizationBase, IPvcbLocalization
    {
        public PvcbLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("CR.PVCB.Localization.SourceFiles");
        }
    }
}
