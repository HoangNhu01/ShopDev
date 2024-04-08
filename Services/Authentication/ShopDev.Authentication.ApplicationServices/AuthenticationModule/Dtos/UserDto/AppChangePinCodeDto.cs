namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class AppChangePinCodeDto
    {
        private string _pinCode = null!;

        /// <summary>
        /// Mã pin hiện tại
        /// </summary>
        public required string PinCode
        {
            get => _pinCode;
            set => _pinCode = value.Trim();
        }

        private string _newPinCode = null!;

        /// <summary>
        /// Mã pin mới
        /// </summary>
        public string NewPinCode
        {
            get => _newPinCode;
            set => _newPinCode = value.Trim();
        }
    }
}
