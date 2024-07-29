namespace ShopDev.Constants.Domain.Auth.RolePermission
{
    public class PermissionContent
    {
        public const string PrefixLocalization = "permission_";

        /// <summary>
        /// Key cha
        /// </summary>
        public string? ParentKey { get; set; }

        /// <summary>
        /// Tên permission cần được transale sang ngôn ngữ tương ứng
        /// </summary>
        public string LName { get; set; }
        public string Icon { get; set; }

        /// <summary>
        /// Khởi tạo tên permission được translate có dạng <see cref="PrefixLocalization"/> + nameof(<paramref name="permissionKey"/>)<br/>
        /// vd: permission_UserMenu viết vào file xml là <code>&lt;text name="permission_UserMenu"&gt;User module&lt;/text&gt;</code>
        /// </summary>
        /// <param name="permissionKey"></param>
        /// <param name="parentKey"></param>
        public PermissionContent(string permissionKey, string icon, string? parentKey = null)
        {
            LName = PrefixLocalization + permissionKey;
            Icon = icon;
            ParentKey = parentKey;
        }
    }
}
