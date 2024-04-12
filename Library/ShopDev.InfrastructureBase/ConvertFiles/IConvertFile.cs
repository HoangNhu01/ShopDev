namespace ShopDev.InfrastructureBase.ConvertFiles
{
    public interface IConvertFile
    {
        Task<Stream> ConvertWordToPdf(Stream input, string fileName);
    }
}
