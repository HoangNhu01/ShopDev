using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.S3Bucket.Constants;

namespace ShopDev.S3Bucket.Exceptions
{
    public class S3BucketException : BaseException
    {
        public S3BucketException(int errorCode)
            : base(errorCode) { }

        public S3BucketException(string? message)
            : base(S3ManagerFileErrorCode.ErrorMessage)
        {
            ErrorMessage = message;
        }

        public S3BucketException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }
    }
}
