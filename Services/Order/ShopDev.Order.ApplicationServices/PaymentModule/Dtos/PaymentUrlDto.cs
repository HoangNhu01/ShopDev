namespace ShopDev.Order.ApplicationServices.PaymentModule.Dtos
{
    public class PaymentUrlDto
    {
        public Guid OrderId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
