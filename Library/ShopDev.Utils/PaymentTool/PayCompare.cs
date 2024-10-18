using System.Globalization;

namespace ShopDev.Utils.PaymentTool
{
    public class PayCompare : IComparer<string>
    {
#pragma warning disable CS8767
        // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public int Compare(string x, string y)
#pragma warning restore CS8767
        // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            if (x == y)
                return 0;
            if (string.IsNullOrEmpty(x))
                return -1;
            if (string.IsNullOrEmpty(y))
                return 1;
            var Compare = CompareInfo.GetCompareInfo("en-US");
            return Compare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
