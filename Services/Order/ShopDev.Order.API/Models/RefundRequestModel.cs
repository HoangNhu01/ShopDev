namespace ShopDev.Order.API.Models
{
    public class RefundRequestModel
    {
        public required string TransactionId { get; set; }
        public double Amount { get; set; }
    }
}
