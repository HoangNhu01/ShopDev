namespace ShopDev.Constants.ErrorCodes
{
    public class InventoryErrorCode : ErrorCode
    {
        protected InventoryErrorCode()
            : base() { }

        public const int ProductNotFound = 9000;
        public const int VariationIsDuplicate = 9001;
    }
}
