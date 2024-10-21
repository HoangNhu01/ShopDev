using System.Linq.Expressions;

namespace ShopDev.Chat.Infrastructure.Repository
{
    public interface IRepository<TEntity, TFilter>
        where TEntity : class
        where TFilter : class
    {
        void SetXRequestId(string? requestId);
        Task SetIndex();
        Task<List<TEntity>> GetAsync();
        Task<TEntity?> GetAsync(string id);
        Task<List<TEntity>> GetAsync(TFilter filter);
        Task<TEntity?> GetFirstOrDefaultAsync(TFilter filter);
        Task DeleteAsync(TFilter filter);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(string id, TEntity entity);
        Task RemoveAsync(string id);
        Task<PagingResult<TEntity>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<bool> AnyAsync(TFilter filter);
        Task<IEnumerable<TEntity>> CreateManyAsync(IEnumerable<TEntity> entities);
		Task<object> ProjectionOneAsync(
			 TFilter filter,
			 Expression<Func<TEntity, object>> projectionExpression
		 );
    }
}
