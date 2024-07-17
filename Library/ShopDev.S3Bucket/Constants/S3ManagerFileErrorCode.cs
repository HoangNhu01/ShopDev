using ShopDev.EntitiesBase.Base;

namespace ShopDev.S3Bucket.Constants
{
    public class S3ManagerFileErrorCode : IErrorCode
    {
        public const int ErrorMessage = 11000;
        public const int UploadMediaError = 11001;
        public const int UploadMediaNotFound = 11002;
        public const int UploadMediaBadRequest = 11003;
        public const int DeleteMediaError = 11004;
        public const int DeleteMediaNotFound = 11005;
        public const int DeleteMediaBadRequest = 11006;
        public const int ReadMediaNotFound = 11007;
        public const int ReadMediaBadRequest = 11008;
        public const int ReadMediaError = 11009;
        public const int CreatBucketError = 11010;
        public const int MoveFileError = 11011;
        public const int MoveFileBadRequest = 11012;
    }
}
