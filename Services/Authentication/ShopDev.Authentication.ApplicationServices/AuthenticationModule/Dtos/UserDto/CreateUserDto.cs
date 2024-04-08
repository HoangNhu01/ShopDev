using ShopDev.ApplicationBase.Common.Validations;
using ShopDev.Constants.Users;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class CreateUserDto
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        private string _username = null!;
        public string Username
        {
            get => _username;
            set => _username = value.Trim();
        }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        private string? _fullName;
        public string? FullName
        {
            get => _fullName;
            set => _fullName = value?.Trim();
        }

        /// <summary>
        /// Avatar người dùng
        /// </summary>
        private string? _avatarImageUrl;
        public string? AvatarImageUrl
        {
            get => _avatarImageUrl;
            set => _avatarImageUrl = value?.Trim();
        }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        [IntegerRange(AllowableValues = new int[] { UserTypes.ADMIN, UserTypes.CUSTOMER })]
        public int? UserType { get; set; }
        public int? Status { get; set; }
        public int CustomerId { get; set; }
        public bool IsPasswordTemp { get; set; }
        public List<int>? RoleIds { get; set; }
    }
}
