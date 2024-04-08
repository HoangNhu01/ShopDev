using ShopDev.ApplicationBase.Common;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface IRoleService
    {
        /// <summary>
        /// Thêm Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        RoleDto Add(CreateRolePermissionDto input);

        /// <summary>
        /// Cập nhật Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        RoleDto Update(UpdateRolePermissionDto input);

        /// <summary>
        /// Find Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RoleDto FindById(int id);

        /// <summary>
        /// Xóa Role
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        /// <summary>
        /// Xem danh sách Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PagingResult<RoleDto> FindAll(FilterRoleDto input);

        /// <summary>
        /// Khoá role
        /// </summary>
        /// <param name="id"></param>
        void ChangeStatus(int id);

        /// <summary>
        /// Lấy danh sách web user có quyền
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> FindAllWebByUser();

        /// <summary>
        /// Lấy danh sách role theo userId
        /// </summary>
        /// <returns></returns>
        IEnumerable<RoleDto> FindRoleByUser(int userId);

        /// <summary>
        /// Danh sách tất cả role không chia quyền theo web
        /// </summary>
        /// <returns></returns>
        IEnumerable<RoleDto> GetAll();
    }
}
