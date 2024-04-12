using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public class AuthenticationDbContextTransient : AuthenticationDbContext
    {
        public AuthenticationDbContextTransient(
            DbContextOptions<AuthenticationDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }
    }
}
