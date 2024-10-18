using CR.InfrastructureBase.Exceptions;

namespace CR.MeeyPartner.Exceptions
{
    public class MeeyPartnerException : BaseException
    {
        /// <summary>
        /// Object cần trả ra
        /// </summary>
        public object? DataValue;

        public MeeyPartnerException(int errorCode)
            : base(errorCode) { }

        public MeeyPartnerException(int errorCode, string? messageLocalize)
            : base(errorCode, messageLocalize) { }

        public MeeyPartnerException(int errorCode, object? dataValue)
            : base(errorCode)
        {
            DataValue = dataValue;
        }
    }
}
