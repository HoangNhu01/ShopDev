using CR.ApplicationBase.Localization;
using CR.PVCB.Constants;
using CR.PVCB.Localization;
using Microsoft.AspNetCore.Http;

namespace CR.PVCB
{
    public class PvcbMapErrorCode : MapErrorCodeBase<PvcbErrorCode>, IPvcbMapErrorCode
    {
        protected override string PrefixError => "error_pvcb_";

        public PvcbMapErrorCode(IPvcbLocalization localization, IHttpContextAccessor httpContext)
            : base(localization, httpContext) { }
    }
}
