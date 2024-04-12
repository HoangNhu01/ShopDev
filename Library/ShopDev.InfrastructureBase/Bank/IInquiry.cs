namespace ShopDev.InfrastructureBase.Bank
{
    public interface IInquiry<out TResponse, in TRequest>
        where TResponse : class
        where TRequest : class
    {
        /// <summary>
        /// Truy vấn tài khoản
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        TResponse Inquiry(TRequest input);
    }
}
