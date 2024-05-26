using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ShopDev.InfrastructureBase.Persistence;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public partial class InventoryDbContext : ApplicationDbContext
    {
        public DbSet<Product> Products { get; init; }
        public DbSet<Spu> Spus { get; init; }

        public InventoryDbContext()
            : base() { }

        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToCollection(nameof(Products));
            modelBuilder
                .Entity<Product>()
                .HasMany(x => x.Spus)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);
            modelBuilder.Entity<Spu>().ToCollection(nameof(Spus));
            modelBuilder.Entity<Spu>().Property(m => m.Version).IsRowVersion();
        }
    }
}
