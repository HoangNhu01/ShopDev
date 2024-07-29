namespace ShopDev.Order.API.Models
{
    public class ConfirmBillModel
    {
        public bool IsSuccess { get; set; }
        public required string Message { get; set; }
    }
}
