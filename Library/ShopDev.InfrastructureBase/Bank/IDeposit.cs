namespace ShopDev.InfrastructureBase.Bank
{
    public interface IDeposit<out TResponse, in TRequest>
        where TResponse : class
        where TRequest : class
    {
        /// <summary>
        /// Chi hộ (Chuyển tiền nội bộ/liên ngân hàng)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        TResponse Deposit(TRequest input);
    }
}
