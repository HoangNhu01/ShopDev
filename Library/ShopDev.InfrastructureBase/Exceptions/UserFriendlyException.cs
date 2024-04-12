namespace ShopDev.InfrastructureBase.Exceptions
{
    /// <summary>
    /// Ngoại lệ cấp người dùng
    /// </summary>
    public class UserFriendlyException : BaseException
    {
        public UserFriendlyException(int errorCode)
            : base(errorCode) { }

        public UserFriendlyException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }

        public UserFriendlyException(int errorCode, params string[] listParam)
            : base(errorCode, listParam) { }
    }
}
