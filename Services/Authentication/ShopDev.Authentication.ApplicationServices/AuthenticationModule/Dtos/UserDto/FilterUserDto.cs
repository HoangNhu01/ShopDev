using ShopDev.ApplicationBase.Common;
using Microsoft.AspNetCore.Mvc;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class FilterUserDto : PagingRequestBaseDto
    {
        [FromQuery(Name = "username")]
        public string? Username { get; set; }

        [FromQuery(Name = "fullname")]
        public string? FullName { get; set; }

        [FromQuery(Name = "status")]
        public int? Status { get; set; }
    }
}
