using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using ShopDev.Authentication.Domain.AuthToken;
using ShopDev.Authentication.Domain.Otps;
using ShopDev.Authentication.Domain.SysVar;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Database;
using ShopDev.Constants.Domain.Auth.Role;
using ShopDev.InfrastructureBase.Persistence;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public partial class AuthenticationDbContext : ApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SysVar> SysVars { get; set; }
        public DbSet<NotificationToken> NotificationTokens { get; set; }
        public DbSet<AuthOtp> AuthOtps { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens { get; set; }

        public AuthenticationDbContext()
            : base() { }

        public AuthenticationDbContext(
            DbContextOptions<AuthenticationDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DbSchemas.Default);
            //modelBuilder
            //    .Entity<NotificationToken>()
            //    .HasOne(e => e.User)
            //    .WithMany()
            //    .HasForeignKey(e => e.UserId);
            //modelBuilder
            //    .Entity<AuthOtp>()
            //    .HasOne(e => e.User)
            //    .WithMany()
            //    .HasForeignKey(e => e.UserId);

            modelBuilder
                .Entity<RolePermission>()
                .HasOne(e => e.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(e => e.RoleId);

            modelBuilder
                .Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<UserRole>().HasOne<User>().WithMany().HasForeignKey(e => e.UserId);

            modelBuilder.Entity<SysVar>();

            modelBuilder.Entity<Role>(entity =>
            {
                entity
                    .Property(e => e.Status)
                    .HasColumnName("Status")
                    .HasDefaultValue(RoleStatus.ACTIVE);
                entity
                    .Property(e => e.PermissionInWeb)
                    .HasColumnName("PermissionInWeb")
                    .HasDefaultValue(PermissionInWebs.Home);
            });

            modelBuilder
                .Entity<User>()
                .HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);
            //modelBuilder
            //	.Entity<Role>()
            //	.HasMany(e => e.UserRoles)
            //	.WithOne(e => e.Role)
            //	.HasForeignKey(e => e.RoleId);
            //modelBuilder
            //    .Entity<User>()
            //    .HasOne(e => e.CoreCustomer)
            //    .WithMany()
            //    .HasForeignKey(e => e.CustomerId);
            modelBuilder.SeedData();
        }
    }
}
