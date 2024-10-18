namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class SetPasswordUserDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public required string Password { get; set; }
        public bool IsPasswordTemp { get; set; }
    }
}
