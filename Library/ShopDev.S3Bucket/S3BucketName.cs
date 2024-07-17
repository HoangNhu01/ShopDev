using CR.ApplicationBase.MultiTenancy;
using CR.S3Bucket.Configs;
using Microsoft.AspNetCore.Http;

namespace CR.S3Bucket
{
    public class S3BucketName : IS3BucketName
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public S3BucketName(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantBucketName()
        {
            object? bucketName = null;
            var tenantFind = _httpContextAccessor
                .HttpContext?.Items
                .TryGetValue(MultiTenancyQuery.BucketName, out bucketName);
            //trường hợp là vào trang quản trị thì tenantId sẽ là null
            return bucketName as string ?? S3Config.BucketName;
        }
    }
}
