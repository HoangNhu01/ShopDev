using Microsoft.AspNetCore.Http;
using ShopDev.ApplicationBase.Localization;
using ShopDev.S3Bucket.Localization;

namespace ShopDev.S3Bucket.Localization
{
    public class S3Localization : LocalizationBase, IS3Localization
    {
        public S3Localization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("CR.S3Bucket.Localization.SourceFiles");
        }
    }
}
