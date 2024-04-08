using Microsoft.AspNetCore.Http;
using ShopDev.InfrastructureBase.Files;
using ShopDev.S3Bucket.Dtos.Delete;
using ShopDev.S3Bucket.Dtos.Media;
using ShopDev.S3Bucket.Dtos.Move;

namespace ShopDev.S3Bucket
{
        /// <summary>
        /// Quản lý file s3
        /// </summary>
        public interface IS3ManagerFile : IManagerFile
        {
                /// <summary>
                /// API cho chức năng tải file lên hệ thống từ người dùng để lưu tạm
                /// </summary>
                /// <param name="files"></param>
                /// <returns></returns>
                Task<ResponseMoveDto> WriteFileAsync(params IFormFile[] files);

                /// <summary>
                /// Api push stream file lên hệ thống, cho các chức năng sinh file và up file lên hệ thống
                /// </summary>
                /// <param name="files"></param>
                /// <returns></returns>
                Task<ResponseMoveDto> WriteStreamFileAsync(params S3StreamFile[] files);

                /// <summary>
                /// Chức năng xóa file trên hệ thống
                /// </summary>
                /// <param name="input"></param>
                /// <returns></returns>
                Task<ResponseContentDto> DeleteFileAsync(RequestDeleteDto input);
        }
}
