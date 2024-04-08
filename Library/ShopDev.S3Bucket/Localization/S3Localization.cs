using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.S3Bucket.Localization
{
    public class S3Localization : LocalizationBase, IS3Localization
    {
        public S3Localization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.S3Bucket.Localization.SourceFiles");
        }
    }
}
