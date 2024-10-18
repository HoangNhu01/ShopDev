using CR.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.MeeyPartner.Localization
{
    public class MeeyPartnerLocalization : LocalizationBase, IMeeyPartnerLocalization
    {
        public MeeyPartnerLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("CR.MeeyPartner.Localization.SourceFiles");
        }
    }
}
