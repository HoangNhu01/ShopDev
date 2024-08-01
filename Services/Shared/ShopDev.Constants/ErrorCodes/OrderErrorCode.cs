namespace ShopDev.Constants.ErrorCodes
{
    public class OrderErrorCode : ErrorCode
    {
        protected OrderErrorCode()
            : base() { }

        public const int ProductNotFound = 10000;
        public const int VariationIsDuplicate = 10001;

        public const int OrderNotFound = 10002;
        public const int ProductIsNotEnough = 10003;
    }
}
