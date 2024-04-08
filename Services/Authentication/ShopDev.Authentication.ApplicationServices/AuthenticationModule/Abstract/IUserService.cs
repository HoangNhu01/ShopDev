using ShopDev.ApplicationBase.Common;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using ShopDev.Authentication.Domain.Users;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface IUserService
    {
        /// <summary>
        /// Tìm kiếm User theo Id (dùng khi gọi authorization)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User FindUserAuthorizatonById(int id);

        /// <summary>
        /// Tìm user theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDto GetById(int id);

        /// <summary>
        /// Validate user admin
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User ValidateAdminUser(string username, string password);

        /// <summary>
        /// Validate app user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User ValidateAppUser(string username, string password);

        /// <summary>
        /// Xem danh sách User CMS (tài khoản SUPER_ADMIN + ADMIN)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PagingResult<UserDto> FindAll(FilterUserDto input);

        /// <summary>
        /// Thêm user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        void CreateUser(CreateUserDto input);

        /// <summary>
        /// Cập nhật thông tin tài khoản
        /// </summary>
        /// <param name="input"></param>
        void Update(UpdateUserDto input);

        /// <summary>
        /// Cập nhật trạng thái tài khoản
        /// </summary>
        /// <param name="id"></param>
        void ChangeStatus(int id);
        void Delete(int id);

        /// <summary>
        /// Set password cho user
        /// </summary>
        /// <param name="input"></param>
        void SetPassword(SetPasswordUserDto input);

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="input"></param>
        void ChangePassword(ChangePasswordDto input);
        void UpdateUserFullname(UpdateFullNameUserDto input);

        /// <summary>
        /// Lưu thông tin ngày gần nhất + thiết bị khi đăng nhập
        /// </summary>
        /// <param name="id"></param>
        void Login(int id);

        /// <summary>
        /// Lấy thông tin lần đăng nhập gần nhất + thiết bị đăng nhập
        /// </summary>
        /// <returns></returns>
        PrivacyInfoDto GetPrivacyInfo();

        /// <summary>
        /// Cập nhật ảnh đại diện cho tài khoản người dùng
        /// </summary>
        /// <param name="s3Key"></param>
        Task UpdateAvatar(string s3Key);

        /// <summary>
        /// Xem giới hạn số lần nhập
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        int GetLimitedInputTurn(string varName);

        /// <summary>
        /// Cập nhật trạng thái user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userStatus"></param>
        void UpdateUserStatus(string userName, int userStatus);

        /// <summary>
        /// Cập nhật trạng thái user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        void UpdateUserStatus(int userId, int userStatus);

        /// <summary>
        /// Cập nhật role theo userId
        /// </summary>
        /// <param name="input"></param>
        void UpdateRole(UpdateRoleDto input);

        /// <summary>
        /// Thông tin tài khoản từ userId token
        /// </summary>
        /// <returns></returns>
        UserDto FindByUser();
    }
}
