namespace ShopDev.Utils.Validation
{
    public class RegexPatterns
    {
        public const string OnlyNumber = @"([0-9]+)";

        /// <summary>
        /// Bắt đầu bằng số 0 và tất cả là số
        /// </summary>
        public const string PhoneNumber = @"^0([0-9]+)";

        //https://stackoverflow.com/questions/19605150/regex-for-password-must-contain-at-least-eight-characters-at-least-one-number-a

        /// <summary>
        /// ít nhất 8 ký tự 1 chữ cái 1 số
        /// </summary>
        public const string Password8Characters1Letter1Number =
            @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[a-zA-Z]).{8,}$";

        /// <summary>
        /// ít nhất 8 ký tự 1 chữ hoa 1 chữ thường 1 số
        /// </summary>
        public const string Password8Characters1Uppercase1Lowercase1Number =
            @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";

        /// <summary>
        /// 8 ký tự có ít nhất 1 chữ hoa 1 chữ thường 1 số 1 ký tự đặc biệt
        /// </summary>
        public const string Password8Characters1Uppercase1Lowercase1Number1Special =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        /// <summary>
        /// Bắt đầu bằng EB hoặc EI và nối với một chuỗi số
        /// </summary>
        public const string ContractCode = @"((EB|EI)[0-9])\d+";
    }
}
