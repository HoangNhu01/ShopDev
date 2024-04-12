namespace ShopDev.InfrastructureBase.Files
{
    /// <summary>
    /// Quản lý file
    /// </summary>
    public interface IManagerFile
    {
        Task<Stream> ReadMediaAsync(string path);
    }
}
