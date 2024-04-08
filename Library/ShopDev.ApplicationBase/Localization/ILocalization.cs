namespace ShopDev.ApplicationBase.Localization
{
    public interface ILocalization
    {
        /// <summary>
        /// Dịch từ key name ra message trong file xml
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        string Localize(string keyName);
    }
}
