using Microsoft.AspNetCore.Http;
using ShopDev.InfrastructureBase.Files.Dtos;

namespace ShopDev.InfrastructureBase.Files
{
    /// <summary>
    /// Quản lý file
    /// </summary>
    public interface IManagerFile
    {
        /// <summary>
        /// Upload file (lưu tạm sau 1 ngày sẽ tự động xóa)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ResponseUploadDto>> UploadAsync(params IFormFile[] input);
    }
}
