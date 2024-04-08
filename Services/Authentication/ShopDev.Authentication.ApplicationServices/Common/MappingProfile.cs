using AutoMapper;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AuthOtpDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using ShopDev.Authentication.Domain.Otps;
using ShopDev.Authentication.Domain.Users;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Role Permission
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<CreateRolePermissionDto, Role>().ReverseMap();
            #endregion

            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<AuthOtpDto, AuthOtp>().ReverseMap();
        }
    }
}
