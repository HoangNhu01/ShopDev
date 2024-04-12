using ShopDev.Authentication.Domain.SysVar;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Users;
using ShopDev.Utils.Security;
using Microsoft.EntityFrameworkCore;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public static class AuthenticationDbContextExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        Password = PasswordHasher.HashPassword("123qwe"),
                        FullName = "admin",
                        UserType = UserTypes.SUPER_ADMIN,
                        Status = UserStatus.ACTIVE
                    }
                );
            modelBuilder
                .Entity<SysVar>()
                .HasData(
                    new SysVar
                    {
                        Id = 2,
                        GrName = "EKYC",
                        VarName = "AGE_MIN",
                        VarValue = "18",
                        VarDesc = "Tuổi nhỏ nhất"
                    }
                );
            modelBuilder
                .Entity<SysVar>()
                .HasData(
                    new SysVar
                    {
                        Id = 3,
                        GrName = "OTP",
                        VarName = "SECOND",
                        VarValue = "60",
                        VarDesc = "Số giây otp hết hạn"
                    }
                );
            modelBuilder
                .Entity<SysVar>()
                .HasData(
                    new SysVar
                    {
                        Id = 4,
                        GrName = "AUTH_MAX_TURN",
                        VarName = "LOGIN_MAX_TURN",
                        VarValue = "5",
                        VarDesc = "Số lượt đăng nhập cho phép"
                    }
                );
            modelBuilder
                .Entity<SysVar>()
                .HasData(
                    new SysVar
                    {
                        Id = 5,
                        GrName = "AUTH_MAX_TURN",
                        VarName = "OTP_MAX_TURN",
                        VarValue = "5",
                        VarDesc = "Số lượt nhập opt cho phép"
                    }
                );
        }
    }
}
