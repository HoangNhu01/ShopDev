using CR.ApplicationBase.Localization;
using CR.FptEkyc.Constants;
using CR.FptEkyc.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.FptEkyc
{
    public class FptEkycMapErrorCode : MapErrorCodeBase<FptEkycErrorCode>, IFptEkycMapErrorCode
    {
        protected override string PrefixError => "error_fpt_ekyc_";

        public FptEkycMapErrorCode(
            IFptEkycLocalization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
