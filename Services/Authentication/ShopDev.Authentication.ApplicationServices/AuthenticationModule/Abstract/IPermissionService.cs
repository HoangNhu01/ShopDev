using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDto;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface IPermissionService
    {
        /// <summary>
        /// Lấy permissionKey của người dùng hiện tại
        /// </summary>
        /// <returns></returns>
        List<string> GetPermission();

        /// <summary>
        /// Lấy danh sách tên quyền theo web
        /// </summary>
        /// <param name="permissionConfig"></param>
        /// <returns></returns>
        IEnumerable<PermissionDetailDto> FindAllPermission(int permissionConfig);

        /// <summary>
        /// Lấy số nhóm quyền, số người dùng theo website
        /// </summary>
        /// <returns></returns>
        IEnumerable<PermissionInWebDto> FindByPermissionInWeb();

        /// <summary>
        /// Lấy permissionKey của người dùng hiện tại (lọc theo website)
        /// </summary>
        /// <param name="permissionInWeb"></param>
        /// <returns></returns>
        List<string> GetPermissionInWeb(int? permissionInWeb);

        /// <summary>
        /// Lấy permissionKey của người dùng bấtt kì
        /// </summary>
        /// <param name="permissionInWeb"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<string> GetPermissionInternalService(int? permissionInWeb, int userId);
    }
}
