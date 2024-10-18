using ShopDev.ApplicationBase.Localization;
using ShopDev.ConvertFile.Constants;
using ShopDev.ConvertFile.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.ConvertFile
{
    public class ConvertFileMapErrorCode
        : MapErrorCodeBase<ConvertFileErrorCodes>,
            IConvertFileMapErrorCode
    {
        protected override string PrefixError => "error_convert_file_";

        public ConvertFileMapErrorCode(
            IConvertFileLocalization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
