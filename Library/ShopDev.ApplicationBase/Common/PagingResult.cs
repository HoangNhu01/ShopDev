namespace ShopDev.ApplicationBase.Common
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
    }
}
