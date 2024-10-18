using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Http;

namespace ShopDev.ConvertFile.Localization
{
    public class ConvertFileLocalization : LocalizationBase, IConvertFileLocalization
    {
        public ConvertFileLocalization(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            LoadDictionary("ShopDev.ConvertFile.Localization.SourceFiles");
        }
    }
}
