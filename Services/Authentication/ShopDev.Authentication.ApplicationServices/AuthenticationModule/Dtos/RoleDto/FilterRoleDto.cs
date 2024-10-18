using ShopDev.ApplicationBase.Common;
using Microsoft.AspNetCore.Mvc;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto
{
    public class FilterRoleDto : PagingRequestBaseDto
    {
        [FromQuery(Name = "userType")]
        public int? UserType { get; set; }

        [FromQuery(Name = "permissionInWeb")]
        public int? PermissionInWeb { get; set; }

        [FromQuery(Name = "status")]
        public int? Status { get; set; }
    }
}
