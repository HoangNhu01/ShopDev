using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.Database;
using ShopDev.InfrastructureBase.Persistence;
using ShopDev.Order.Domain.Order;
using ShopDev.Order.Domain.Shipment;

namespace ShopDev.Order.Infrastructure.Persistence
{
    public partial class OrderDbContext : ApplicationDbContext
    {
        public DbSet<OrderGen> Orders { get; init; }
        public DbSet<OrderDetail> OrderDetails { get; init; }
        public DbSet<OrderProcess> OrderProcesses { get; init; }

        public OrderDbContext()
            : base() { }

        public OrderDbContext(
            DbContextOptions<OrderDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DbSchemas.SDOrder);
            modelBuilder
                .Entity<OrderDetail>()
                .OwnsOne(
                    pr => pr.Product,
                    ownerNav =>
                    {
                        ownerNav.Property(ad => ad.Name).HasColumnType("nvarchar(max)");
                        ownerNav.Property(ad => ad.Title).HasColumnType("nvarchar(max)");
                        ownerNav.OwnsMany(
                            s => s.Spus,
                            o =>
                            {
                                o.Property(d => d.Name).HasColumnType("nvarchar(max)");
                                o.ToJson();
                            }
                        );
                        ownerNav.ToJson();
                    }
                );
            modelBuilder
                .Entity<OrderGen>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            modelBuilder
                .Entity<OrderGen>()
                .HasMany(x => x.OrderProcesses)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            modelBuilder.Entity<OrderGen>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValue(Guid.NewGuid());
            });
        }
    }
}
