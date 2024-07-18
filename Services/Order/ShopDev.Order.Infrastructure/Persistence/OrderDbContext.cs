using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopDev.Constants.Database;
using ShopDev.InfrastructureBase.Persistence;

namespace ShopDev.Order.Infrastructure.Persistence
{
    public partial class OrderDbContext : ApplicationDbContext
    {
        public OrderDbContext()
            : base() { }

        public OrderDbContext(
            DbContextOptions<OrderDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DbSchemas.SDInventory);
            //modelBuilder.Entity<Product>().ToCollection(nameof(Products));
        }
    }
}
