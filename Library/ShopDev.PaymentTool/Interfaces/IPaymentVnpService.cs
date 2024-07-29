namespace ShopDev.PaymentTool.Interfaces
{
    public interface IPaymentVnpService : IPaymentToolService
    {
        string CreateRequestUrl(string baseUrl, string vnp_HashSecret);
        void AddResponseData(string key, string value);
        string? GetResponseDataByKey(string key);
        string GetResponseData();
    }
}
