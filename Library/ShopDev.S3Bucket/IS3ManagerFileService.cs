using Microsoft.AspNetCore.Http;
using ShopDev.InfrastructureBase.Files;
using ShopDev.InfrastructureBase.Files.Dtos;

namespace ShopDev.S3Bucket
{
    /// <summary>
    /// Quản lý file s3
    /// </summary>
    public interface IS3ManagerFileService : IManagerFile
    {
        /// <summary>
        /// Đọc file
        /// </summary>
        /// <param name="s3Key"></param>
        /// <returns></returns>
        Task<Stream> ReadAsync(string s3Key);
        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="s3Key"></param>
        /// <returns></returns>
        Task<DownloadFileDto> DownloadAsync(string s3Key);
        /// <summary>
        /// Move file (chuyển từ tạm sang lưu thật)
        /// </summary>
        /// <param name="s3key"></param>
        /// <returns></returns>
        Task<List<ResponseUploadDto>> MoveAsync(params string[] s3key);
        /// <summary>
        /// Xóa file
        /// </summary>
        /// <param name="s3key"></param>
        /// <returns></returns>
        Task DeleteAsync(params string[] s3key);

        /// <summary>
        /// Upload file thẳng bỏ qua move
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ResponseUploadDto>> UploadFileAsync(params IFormFile[] input);
    }
}
