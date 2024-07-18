namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDto
{
    public class PermissionDetailDto
    {
        /// <summary>
        /// PermissionKey
        /// </summary>
        public string Key { get; set; } = null!;
        public string? ParentKey { get; set; }
        public string? Label { get; set; }
        public string? Icon { get; set; }
    }
}
