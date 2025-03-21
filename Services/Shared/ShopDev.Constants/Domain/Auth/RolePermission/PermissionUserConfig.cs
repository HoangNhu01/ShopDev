using ShopDev.Constants.Domain.Auth.RolePermission;
using ShopDev.Constants.Domain.Auth.RolePermission.Constant;
using ShopDev.Constants.RolePermission.Constant;

namespace ShopDev.Constants.RolePermission
{
    public static partial class PermissionConfig
    {
        public static readonly Dictionary<string, PermissionContent> UserConfigs =
            new()
            {
                //Tổng quan
                {
                    PermissionKeys.UserMenuDashboard,
                    new(nameof(PermissionKeys.UserMenuDashboard), PermissionIcons.IconDefault)
                },
                //Phân quyền website
                {
                    PermissionKeys.UserMenuPermission,
                    new(nameof(PermissionKeys.UserMenuPermission), PermissionIcons.IconDefault)
                },
                {
                    PermissionKeys.UserTablePermission,
                    new(
                        nameof(PermissionKeys.UserTablePermission),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuPermission
                    )
                },
                {
                    PermissionKeys.UserButtonPermissionSetting,
                    new(
                        nameof(PermissionKeys.UserButtonPermissionSetting),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuPermission
                    )
                },
                {
                    PermissionKeys.UserButtonPermissionAdd,
                    new(
                        nameof(PermissionKeys.UserButtonPermissionAdd),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserButtonPermissionSetting
                    )
                },
                {
                    PermissionKeys.UserTableRolePermission,
                    new(
                        nameof(PermissionKeys.UserTableRolePermission),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserButtonPermissionSetting
                    )
                },
                {
                    PermissionKeys.UserButtonRolePermissionUpdate,
                    new(
                        nameof(PermissionKeys.UserButtonRolePermissionUpdate),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserButtonPermissionSetting
                    )
                },
                {
                    PermissionKeys.UserButtonRolePermissionLock,
                    new(
                        nameof(PermissionKeys.UserButtonRolePermissionLock),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserButtonPermissionSetting
                    )
                },
                {
                    PermissionKeys.UserButtonRolePermissionDelete,
                    new(
                        nameof(PermissionKeys.UserButtonRolePermissionDelete),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserButtonPermissionSetting
                    )
                },
                //Quản lý tài khoản
                {
                    PermissionKeys.UserMenuAccountManager,
                    new(nameof(PermissionKeys.UserMenuAccountManager), PermissionIcons.IconDefault)
                },
                {
                    PermissionKeys.UserButtonAccountManagerAdd,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagerAdd),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                {
                    PermissionKeys.UserTableAccountManager,
                    new(
                        nameof(PermissionKeys.UserTableAccountManager),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                {
                    PermissionKeys.UserButtonAccountManagerUpdatePermission,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagerUpdatePermission),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                {
                    PermissionKeys.UserButtonAccountManagerUpdatePassword,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagerUpdatePassword),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                {
                    PermissionKeys.UserButtonAccountManagerLock,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagerLock),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                {
                    PermissionKeys.UserButtonAccountManagerDelete,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagerDelete),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuAccountManager
                    )
                },
                // Báo cáo
                {
                    PermissionKeys.UserMenuExportReport,
                    new(nameof(PermissionKeys.UserMenuExportReport), PermissionIcons.IconDefault)
                },
                {
                    PermissionKeys.UserButtonAccountManagementReport,
                    new(
                        nameof(PermissionKeys.UserButtonAccountManagementReport),
                        PermissionIcons.IconDefault,
                        PermissionKeys.UserMenuExportReport
                    )
                },
            };
    }
}
