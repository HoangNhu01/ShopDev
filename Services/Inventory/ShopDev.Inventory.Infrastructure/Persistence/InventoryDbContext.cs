using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ShopDev.Constants.Database;
using ShopDev.InfrastructureBase.Persistence;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Domain.Shops;

namespace ShopDev.Authentication.Infrastructure.Persistence
{
    public partial class InventoryDbContext : ApplicationDbContext
    {
        public DbSet<Product> Products { get; init; }
        public DbSet<Spu> Spus { get; init; }
        public DbSet<Shop> Shops { get; init; }
        public DbSet<Category> Categories { get; init; }
        public DbSet<CategoryType> CategoryTypes { get; init; }

        public InventoryDbContext()
            : base() { }

        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options, httpContextAccessor) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DbSchemas.SDInventory);
            //modelBuilder.Entity<Product>().ToCollection(nameof(Products));
            modelBuilder
                .Entity<Product>()
                .HasMany(x => x.Spus)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);

            modelBuilder
                .Entity<Shop>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Shop)
                .HasForeignKey(x => x.ShopId);

            modelBuilder
                .Entity<CategoryType>()
                .HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);

            //modelBuilder.Entity<Product>(entity =>
            //{
            //    entity.Property(x => x.Variations).HasColumnType("jsonb");
            //    entity.Property(x => x.Attributes).HasColumnType("jsonb");
            //});
            modelBuilder
                .Entity<Product>()
                .OwnsMany(pr => pr.Attributes, ownerNav => ownerNav.ToJson())
                .OwnsMany(
                    pr => pr.Variations,
                    ownerNav =>
                    {
                        ownerNav.ToJson();
                    }
                )
                .HasQueryFilter(x => !x.Deleted);
            //modelBuilder.Entity<Spu>().ToCollection(nameof(Spus));
            JsonSerializerOptions options =
                new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
            modelBuilder
                .Entity<Spu>()
                .Property(e => e.Index)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, options),
                    v => JsonSerializer.Deserialize<List<int>>(v, options) ?? new()
                );
            modelBuilder.Entity<Spu>().Property(m => m.Version).IsRowVersion();
            modelBuilder.Entity<Spu>().HasQueryFilter(x => !x.Deleted);

            modelBuilder
                .Entity<Spu>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Spus)
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<CategoryType>().HasQueryFilter(x => !x.Deleted);
            modelBuilder
                .Entity<CategoryType>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.ProductId);

            modelBuilder
                .Entity<CategoryType>()
                .HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);
            //modelBuilder.Entity<Category>().ToCollection(nameof(Categories));
            base.OnModelCreating(modelBuilder);
        }
    }
}
