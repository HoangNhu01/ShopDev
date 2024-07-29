namespace eShopSolution.ViewModels.Sales
{
    public class GetUrlModel
    {
        public Guid OrderId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
