namespace ShopDev.Constants.ErrorCodes
{
    public class OrderErrorCode : ErrorCode
    {
        protected OrderErrorCode()
            : base() { }

        public const int ProductNotFound = 10000;
        public const int VariationIsDuplicate = 10001;
    }
}
