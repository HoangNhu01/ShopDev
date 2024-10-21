using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using Pipelines.Sockets.Unofficial.Arenas;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Header;
using ShopDev.EntitiesBase.Base;
using ShopDev.Utils.DataUtils;

namespace ShopDev.Chat.Infrastructure.Repository
{
    public abstract class Repository<TEntity, TFilter, TUpdate> : IRepository<TEntity, TFilter>
        where TEntity : class, IEntity<string>
        where TFilter : FilterDefinition<TEntity>
        where TUpdate : UpdateDefinition<TEntity>
    {
        protected readonly ILogger _logger;
        protected readonly int? UserId;
        protected readonly int? TenantId;
        protected readonly IMongoCollection<TEntity> _collection;
        protected StringValues XRequestId;
        protected IMongoDatabase _database;

        public Repository(
            ILogger logger,
            IHttpContextAccessor httpContextAccessor,
            IMongoDatabase database,
            string collectionName
        )
        {
            if (
                httpContextAccessor
                    .HttpContext?.Request
                    .Headers.TryGetValue(HeaderNames.XRequestId, out var requestId) == true
            )
            {
                XRequestId = requestId;
                Serilog.Log.ForContext<ILogger>().ForContext("XRequestId", requestId);
            }
            _logger = logger;
            _database = database;
            _collection = database.GetCollection<TEntity>(collectionName);
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                UserId = userId;
            }
        }

        public void SetXRequestId(string? requestId)
        {
            if (XRequestId == StringValues.Empty || requestId == null)
            {
                return;
            }
            XRequestId = new([requestId]);
            Serilog.Log.ForContext<ILogger>().ForContext("XRequestId", XRequestId);
        }

        protected void CreateAudit(TEntity entity)
        {
            if (entity is ICreatedBy createdEntity)
            {
                createdEntity.CreatedDate = DateTimeUtils.GetDate();
                createdEntity.CreatedBy = UserId;
            }
        }

        protected void UpdateAudit(TEntity entity)
        {
            if (entity is IModifiedBy modifiedEntity)
            {
                modifiedEntity.ModifiedDate = DateTimeUtils.GetDate();
                modifiedEntity.ModifiedBy = UserId;
            }
            DeleteAudit(entity); //trường hợp soft delete dùng update
        }

        protected void DeleteAudit(TEntity entity)
        {
            if (entity is ISoftDeleted softDeletedEntity && softDeletedEntity.Deleted)
            {
                softDeletedEntity.DeletedDate = DateTimeUtils.GetDate();
                softDeletedEntity.DeletedBy = UserId;
            }
        }

        /// <summary>
        /// Tạo filter mặc định, trường hợp có tenantId sẽ filter thêm tenantId
        /// </summary>
        /// <returns></returns>
        protected FilterDefinition<TEntity> CreateFilter()
        {
            var filterBuilder = Builders<TEntity>.Filter;
            var filter = filterBuilder.Empty;
            return filter;
        }

        public async Task<List<TEntity>> GetAsync(TFilter filter)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(GetAsync),
                nameof(filter),
                filter.ToJson()
            );
            return await _collection.Find(filterAnd).ToListAsync();
        }

        public async Task<List<TEntity>> GetAsync()
        {
            var filter = CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(GetAsync),
                nameof(filter),
                filter.ToJson()
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<TEntity?> GetAsync(string id)
        {
            var filter = CreateFilter();
            var filterBuilder = Builders<TEntity>.Filter;
            filter = filterBuilder.And(filter, filterBuilder.Eq("_id", new ObjectId(id)));
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(GetAsync),
                nameof(filter),
                filter.ToJson()
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(TFilter filter)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(GetFirstOrDefaultAsync),
                nameof(filter),
                filter.ToJson()
            );
            return await _collection.Find(filterAnd).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(TFilter filter)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(AnyAsync),
                nameof(filter),
                filter.ToJson()
            );
            var count = await _collection.CountDocumentsAsync(filterAnd);
            return count > 0;
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            CreateAudit(entity);
            _logger.LogInformation(
                "{Repo}->{Method}: {Entity} = {EntityData}",
                GetType().FullName,
                nameof(CreateAsync),
                nameof(entity),
                JsonSerializer.Serialize(entity)
            );
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> CreateManyAsync(IEnumerable<TEntity> entities)
        {
            entities.AsParallel().ForAll(x => CreateAudit(entity: x));
            _logger.LogInformation(
                "{Repo}->{Method}: {Entity} = {EntityData}",
                GetType().FullName,
                nameof(CreateAsync),
                nameof(entities),
                JsonSerializer.Serialize(entities)
            );
            await _collection.InsertManyAsync(entities);
            return entities;
        }

        public async Task UpdateAsync(string id, TEntity entity)
        {
            var filter = CreateFilter();
            var filterBuilder = Builders<TEntity>.Filter;
            filter = filterBuilder.And(filter, filterBuilder.Eq("_id", new ObjectId(id)));
            UpdateAudit(entity);
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}, {Entity} = {EntityData}",
                GetType().FullName,
                nameof(UpdateAsync),
                nameof(filter),
                filter.ToJson(),
                nameof(entity),
                JsonSerializer.Serialize(entity)
            );
            entity.Id = id;
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(string id)
        {
            var filter = CreateFilter();
            var filterBuilder = Builders<TEntity>.Filter;
            filter = filterBuilder.And(filter, filterBuilder.Eq("_id", new ObjectId(id)));
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(RemoveAsync),
                nameof(filter),
                filter.ToJson()
            );
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<PagingResult<TEntity>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation(
                "{Repo}->{Method}: {PageNumber} = {PageNumberData}, {PageSize} = {PageSizeData}",
                GetType().FullName,
                nameof(GetPaginatedAsync),
                nameof(pageNumber),
                pageNumber,
                nameof(pageSize),
                pageSize
            );
            return await GetPaginatedAsync(CreateFilter(), pageNumber, pageSize);
        }

        protected async Task<PagingResult<TEntity>> GetPaginatedAsync(
            FilterDefinition<TEntity> filter,
            int pageNumber,
            int pageSize
        )
        {
            filter = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}, {PageNumber} = {PageNumberValue}, {PageSize} = {PageSizeValue}",
                GetType().FullName,
                nameof(GetPaginatedAsync),
                nameof(filter),
                filter.ToJson(),
                nameof(pageNumber),
                pageNumber,
                nameof(pageSize),
                pageSize
            );
            var totalItems = await _collection.CountDocumentsAsync(filter);
            var items = await _collection
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagingResult<TEntity> { Items = items, TotalItems = totalItems };
        }

        /// <summary>
        /// Lấy phân trang theo limit offset, filter mặc định theo tenantId nếu tenantId khác null
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected async Task<PagingResult<TEntity>> GetPaginatedLimitOffsetAsync(
            FilterDefinition<TEntity> filter,
            int offset,
            int limit
        )
        {
            filter = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}, {Offset} = {OffsetValue}, {Limit} = {LimitValue}",
                GetType().FullName,
                nameof(GetPaginatedLimitOffsetAsync),
                nameof(filter),
                filter.ToJson(),
                nameof(offset),
                offset,
                nameof(limit),
                limit
            );
            var totalItems = await _collection.CountDocumentsAsync(filter);
            var items = await _collection.Find(filter).Skip(offset).Limit(limit).ToListAsync();

            return new PagingResult<TEntity> { Items = items, TotalItems = totalItems };
        }

        /// <summary>
        /// Xoá theo filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TFilter filter)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(DeleteAsync),
                nameof(filter),
                filter.ToJson()
            );

            await _collection.DeleteOneAsync(filterAnd);
        }

        /// <summary>
        /// Update nhiều document theo filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        protected async Task UpdateManyAsync(TFilter filter, TUpdate update)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}, {Update} = {UpdateData}",
                GetType().FullName,
                nameof(DeleteAsync),
                nameof(filter),
                filter.ToJson(),
                nameof(update),
                update.ToJson()
            );
            await _collection.UpdateManyAsync(filterAnd, update);
        }

        /// <summary>
        /// Update một document theo filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        protected async Task UpdateOneAsync(TFilter filter, TUpdate update)
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}, {Update} = {UpdateData}",
                GetType().FullName,
                nameof(DeleteAsync),
                nameof(filter),
                filter.ToJson(),
                nameof(update),
                update.ToJson()
            );
            await _collection.UpdateOneAsync(filterAnd, update);
        }

        public async Task<object> ProjectionOneAsync(
            TFilter filter,
            Expression<Func<TEntity, object>> projectionExpression
        )
        {
            var filterAnd = filter & CreateFilter();
            _logger.LogInformation(
                "{Repo}->{Method}: {Filter} = {FilterData}",
                GetType().FullName,
                nameof(DeleteAsync),
                nameof(filter),
                filter.ToJson()
            );
            var projection = Builders<TEntity>.Projection.Expression(projectionExpression);
            return (await _collection.Find(filter).Project(projection).FirstOrDefaultAsync());
        }

        public abstract Task SetIndex();
    }
}

public class PagingResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public long TotalItems { get; set; }
}

/// <summary>
/// Request phân trang theo limit và offset
/// </summary>
public class PagingRequestBaseLimitOffset
{
    /// <summary>
    /// Lấy bao nhiêu bản ghi
    /// </summary>
    [FromQuery(Name = "limit")]
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Bỏ qua bao nhiêu bản ghi
    /// </summary>
    [FromQuery(Name = "offSet")]
    public int OffSet { get; set; } = 0;
}
