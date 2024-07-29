namespace ShopDev.Constants.RolePermission.Constant
{
    public static partial class PermissionKeys
    {
        //Tổng quan
        public const string UserMenuDashboard = $"{Menu}user_menu_dashboard";

        //Phân quyền website
        public const string UserMenuPermission = $"{Menu}user_permission";
        public const string UserTablePermission = $"{Table}user_permission";
        public const string UserButtonPermissionSetting = $"{Button}user_permission_setting";
        public const string UserButtonPermissionAdd = $"{Button}user_permission_add";
        public const string UserTableRolePermission = $"{Table}user_role_permission";
        public const string UserButtonRolePermissionUpdate = $"{Button}user_role_permission_update";
        public const string UserButtonRolePermissionLock = $"{Button}user_role_permission_lock";
        public const string UserButtonRolePermissionDelete = $"{Button}user_role_permission_delete";

        //Quản lý tài khoản
        public const string UserMenuAccountManager = $"{Menu}account_manager";
        public const string UserButtonAccountManagerAdd = $"{Button}add_account";
        public const string UserTableAccountManager = $"{Table}account";
        public const string UserButtonAccountManagerUpdatePermission =
            $"{Button}update_permission_account";
        public const string UserButtonAccountManagerUpdatePassword =
            $"{Button}update_password_account";
        public const string UserButtonAccountManagerLock = $"{Button}lock_account";
        public const string UserButtonAccountManagerDelete = $"{Button}delete_account";

        // Báo cáo
        public const string UserMenuExportReport = $"{Menu}export_report";
        public const string UserButtonAccountManagementReport = $"{Button}account_manager_report";
    }
}
