using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.AuthorizationEntities;
using ShopDev.Utils.DataUtils;

namespace ShopDev.InfrastructureBase.Persistence
{
    public abstract class ApplicationDbContext : DbContext
    {
        protected readonly IHttpContextAccessor _httpContextAccessor = null!;
        protected readonly int? UserId = null;

        public ApplicationDbContext() { }

        public ApplicationDbContext(
            DbContextOptions options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            var claims = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                UserId = userId;
            }
        }
        private void CheckAudit()
        {
            ChangeTracker.DetectChanges();
            var added = ChangeTracker
                .Entries()
                .Where(t => t.State == EntityState.Added)
                .Select(t => t.Entity)
                .AsParallel();

            added.ForAll(entity =>
            {
                if (entity is ICreatedBy createdEntity && createdEntity.CreatedBy == null)
                {
                    createdEntity.CreatedDate = DateTimeUtils.GetDate();
                    createdEntity.CreatedBy = UserId;
                }
            });

            var modified = ChangeTracker
                .Entries()
                .Where(t => t.State == EntityState.Modified)
                .Select(t => t.Entity)
                .AsParallel();
            modified.ForAll(entity =>
            {
                if (entity is IModifiedBy modifiedEntity && modifiedEntity.ModifiedBy == null)
                {
                    modifiedEntity.ModifiedDate = DateTimeUtils.GetDate();
                    modifiedEntity.ModifiedBy = UserId;
                }
                if (
                    entity is ISoftDeleted softDeletedEntity
                    && softDeletedEntity.Deleted
                    && softDeletedEntity.DeletedBy == null
                )
                {
                    softDeletedEntity.DeletedDate = DateTimeUtils.GetDate();
                    softDeletedEntity.DeletedBy = UserId;
                }
            });
        }

        public override int SaveChanges()
        {
            CheckAudit();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            CheckAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CheckAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default
        )
        {
            CheckAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeleted).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var deletedProperty = Expression.Property(
                        parameter,
                        nameof(ISoftDeleted.Deleted)
                    );
                    var compareExpression = Expression.Equal(
                        deletedProperty,
                        Expression.Constant(false)
                    );
                    var lambda = Expression.Lambda(compareExpression, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
