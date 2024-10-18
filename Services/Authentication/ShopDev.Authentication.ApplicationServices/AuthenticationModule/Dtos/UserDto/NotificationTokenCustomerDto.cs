namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class NotificationTokenCustomerDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Số điện thoại KHCN
        /// </summary>
        public string? PersonalPhone { get; set; }

        /// <summary>
        /// Kiểm tra KHCN đã ekyc
        /// </summary>
        public bool PersonalIsEkycStatus { get; set; }

        /// <summary>
        /// Kiểm tra đã face match
        /// </summary>
        public bool IsFaceMatch { get; set; }
    }
}
