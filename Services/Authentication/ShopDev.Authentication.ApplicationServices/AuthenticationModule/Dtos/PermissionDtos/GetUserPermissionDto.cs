using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDtos
{
    public class GetUserPermissionDto
    {
        public int UserType { get; set; }
        public int UserId { get; set; }
        public int? PermissionInWeb { get; set; }

    }
}
