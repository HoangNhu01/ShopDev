namespace ShopDev.InfrastructureBase.Exceptions
{
    public class BaseException : Exception
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public readonly int ErrorCode;

        /// <summary>
        /// Chuỗi cần localize sẽ tra trong từ điển, nếu có truyền chuỗi này thì
        /// sẽ không lấy message của error nữa mà lấy theo chuỗi này
        /// </summary>
        public readonly string? MessageLocalize;

        /// <summary>
        /// Chuỗi cần trả ra (Không localize chỉ dùng chung 1 biến ErrorCode)
        /// </summary>
        public string? ErrorMessage;

        /// <summary>
        /// Mảng chuỗi cần chả
        /// </summary>
        public string[]? ListParam;

        public BaseException(int errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        public BaseException(int errorCode, string? messageLocalize)
            : base()
        {
            ErrorCode = errorCode;
            MessageLocalize = messageLocalize;
        }

        public BaseException(int errorCode, params string[] listParam)
            : base()
        {
            ErrorCode = errorCode;
            ListParam = listParam;
        }
    }
}
