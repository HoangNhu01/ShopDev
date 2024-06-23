namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class UserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Avatar người dùng
        /// </summary>
        public string? AvatarImageUri { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public int? UserType { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        public bool IsPasswordTemp { get; set; }
        public IEnumerable<string>? RoleNames { get; set; }
        public IEnumerable<int>? RoleIds { get; set; }
        public string? PinCode { get; set; }
        public bool IsTempPin { get; set; }
    }
}
