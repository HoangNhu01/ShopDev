using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopDev.Authentication.Domain.AuthToken;
using ShopDev.Authentication.Domain.Otps;
using ShopDev.Authentication.Domain.SysVar;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Database;
using ShopDev.Constants.Role;
using ShopDev.Constants.Users;
using ShopDev.InfrastructureBase.Persistence;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public partial class AuthenticationDbContext : ApplicationDbContext<User>
    {
        public override DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SysVar> SysVars { get; set; }
        public DbSet<NotificationToken> NotificationTokens { get; set; }
        public DbSet<AuthOtp> AuthOtps { get; set; }

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
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.IsTempPin).HasColumnName("IsTempPin").HasDefaultValue(false);
                entity
                    .Property(e => e.Status)
                    .HasColumnName("Status")
                    .HasDefaultValue(UserStatus.ACTIVE);
                entity
                    .Property(e => e.UserType)
                    .HasColumnName("UserType")
                    .HasDefaultValue(UserTypes.ADMIN);
                entity
                    .Property(e => e.LoginFailCount)
                    .HasColumnName("LoginFailCount")
                    .HasDefaultValue(0);
                entity
                    .Property(e => e.DateTimeLoginFailCount)
                    .HasColumnName("DateTimeLoginFailCount");
            });
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

            //modelBuilder
            //	.Entity<RolePermission>()
            //	.HasOne(e => e.Role)
            //	.WithMany(x => x.RolePermissions)
            //	.HasForeignKey(e => e.RoleId);

            modelBuilder
                .Entity<UserRole>()
                .HasOne<Role>()
                .WithMany()
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

            //modelBuilder
            //	.Entity<User>()
            //	.HasMany(e => e.UserRoles)
            //	.WithOne(e => e.User)
            //	.HasForeignKey(e => e.UserId);
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
            base.OnModelCreating(modelBuilder);
        }
    }
}
