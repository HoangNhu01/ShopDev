namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    /// <summary>
    /// Tài khoản thực hiện việc nào đó
    /// </summary>
    public class UserByDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public int UserType { get; set; }
    }
}
