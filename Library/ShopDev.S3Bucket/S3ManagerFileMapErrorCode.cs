using ShopDev.ApplicationBase.Localization;
using ShopDev.S3Bucket.Constants;
using ShopDev.S3Bucket.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.S3Bucket
{
    public class S3ManagerFileMapErrorCode
        : MapErrorCodeBase<S3ManagerFileErrorCode>,
            IS3MapErrorCode
    {
        protected override string PrefixError => "error_s3_";

        public S3ManagerFileMapErrorCode(
            IS3Localization localization,
            IHttpContextAccessor httpContext
        )
            : base(localization, httpContext) { }
    }
}
