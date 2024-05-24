using Microsoft.EntityFrameworkCore;
using ShopDev.Authentication.Domain.SysVar;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Users;
using ShopDev.Utils.Security;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public static class InventoryDbContextExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder) { }
    }
}
