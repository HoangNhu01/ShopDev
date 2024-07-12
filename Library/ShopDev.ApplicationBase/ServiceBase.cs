using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;

namespace ShopDev.ApplicationBase
{
    public abstract class ServiceBase<TDbContext>
        where TDbContext : DbContext
    {
        protected readonly ILogger _logger;
        protected readonly TDbContext _dbContext;
        protected readonly LocalizationBase _localization;
        protected readonly IHttpContextAccessor _httpContext;
        protected readonly IMapper _mapper;
        protected readonly IMapErrorCode? _mapErrorCode;

        protected ServiceBase(ILogger logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = httpContext.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
            _localization =
                httpContext.HttpContext!.RequestServices.GetRequiredService<LocalizationBase>();
            _mapper = _httpContext.HttpContext!.RequestServices.GetRequiredService<IMapper>();
        }

        protected ServiceBase(
            ILogger logger,
            IHttpContextAccessor httpContext,
            TDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _localization = localizationBase;
            _mapper = mapper;
        }

        protected ServiceBase(
            ILogger logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            TDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _localization = localizationBase;
            _mapper = mapper;
            _mapErrorCode = mapErrorCode;
        }

        #region Thay thế cho tầng Generic repository
        protected IQueryable<Entity> GetEntities<Entity>(Expression<Func<Entity, bool>>? expression)
            where Entity : class
        {
            IQueryable<Entity> query = _dbContext.Set<Entity>();
            if (expression is not null)
            {
                query = query.Where(expression);
            }
            return query;
        }

        protected TEntity? FindEntity<TEntity>(
            Expression<Func<TEntity, bool>> expression,
            bool isTracking = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
        )
            where TEntity : class
        {
            IQueryable<TEntity> entities = _dbContext.Set<TEntity>();
            if (isTracking)
            {
                entities.AsNoTracking();
            }
            return entities.FirstOrDefault(expression);
        }

        protected IQueryable<TResult> GetIQueryableResult<TResult, TEntity>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true
        )
            where TResult : class
            where TEntity : class
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include is not null)
            {
                query = include(query);
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }
            return query.Select(selector);
        }
        #endregion

        /// <summary>
        /// Update các item trong
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="items"></param>
        /// <param name="inputItems"></param>
        /// <param name="comparer"></param>
        /// <param name="updateAction"></param>
        public static void UpdateItems<TEntity, TDto>(
            List<TEntity> items,
            List<TDto> inputItems,
            Func<TEntity, TDto, bool> comparer,
            Action<TEntity, TDto> updateAction
        )
            where TEntity : class
        {
            foreach (var item in items)
            {
                var inputItem = inputItems.Find(x => comparer(item, x));
                if (inputItem is null && item.GetType().GetProperty("Deleted") is not null)
                {
                    item.GetType().GetProperty("Deleted")!.SetValue(item, true);
                }
                else
                {
                    updateAction(item, inputItem!);
                    inputItems.Remove(inputItem!);
                }
            }
        }

        /// <summary>
        /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì <br/>
        /// Input: <paramref name="keyName"/> = "error_System" <br/>
        /// Return: "Error System" hoặc "Lỗi" tuỳ theo request ngôn ngữ đang là gì ví dụ ở đây là "en" và "VI"
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        protected string L(string keyName)
        {
            return _localization.Localize(keyName);
        }

        /// <summary>
        /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì và dùng <c>string.Format()</c> để format chuỗi<br/>
        /// Ví dụ có thẻ <c>&lt;text name="hello"&gt;Xin chào {0}, {1} tuổi&lt;/text&gt;</c> trong file <c>xml</c> <br/>
        /// Input: <paramref name="keyName"/> = "hello" <paramref name="values"/> = ["Minh", 20] <br/>
        /// Return: "Xin chào Minh, 20 tuổi"
        /// </summary>
        /// <returns></returns>
        protected string L(string keyName, params string[] values)
        {
            return string.Format(_localization.Localize(keyName), values);
        }
    }
}
