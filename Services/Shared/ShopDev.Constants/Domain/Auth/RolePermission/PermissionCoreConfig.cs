using ShopDev.Constants.Domain.Auth.RolePermission;
using ShopDev.Constants.Domain.Auth.RolePermission.Constant;
using ShopDev.Constants.RolePermission.Constant;

namespace ShopDev.Constants.RolePermission
{
    public static partial class PermissionConfig
    {
        public static readonly Dictionary<string, PermissionContent> CoreConfigs =
            new()
            {
                #region Tổng quan
                {
                    PermissionKeys.CoreMenuDashboard,
                    new(nameof(PermissionKeys.CoreMenuDashboard), PermissionIcons.IconDefault)
                },
                #endregion
            };
    }
}
