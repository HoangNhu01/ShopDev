using CR.ApplicationBase.Localization;
using CR.MeeyPartner.Constants;
using CR.MeeyPartner.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.MeeyPartner
{
    public class MeeyPartnerMapErrorCode
        : MapErrorCodeBase<MeeyPartnerErrorCode>,
            IMeeyPartnerMapErrorCode
    {
        protected override string PrefixError => "error_meey_partner_";

        public MeeyPartnerMapErrorCode(
            IMeeyPartnerLocalization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
