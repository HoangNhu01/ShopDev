using System.Globalization;
using System.Linq.Expressions;
using ShopDev.Utils.Linq.Const;

namespace ShopDev.Utils.Linq
{
    public static class LinqUtils
    {
        /// <summary>
        /// sắp xếp linh động
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderDynamic<TEntity>(
            this IQueryable<TEntity> query,
            IList<string>? sort
        )
            where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "o");
            var propertyId = typeof(TEntity).GetProperty("Id");
            if (propertyId != null)
            {
                query = query.OrderByDescending(
                    Expression.Lambda<Func<TEntity, object>>(
                        Expression.Convert(Expression.Property(param, propertyId), typeof(object)),
                        param
                    )
                );
            }

            if (sort == null)
            {
                return query;
            }

            for (var i = 0; i < sort.Count; i++)
            {
                var item = sort[i].Split('-');
                if (item.Length != 2)
                {
                    continue;
                }
                var field = item[0].Trim();
                var order = item[1].Trim();

                //chuyển đổi camelCase thành PacalCase
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string pascalCaseField = textInfo.ToTitleCase(field[..1]) + field[1..];

                var property = typeof(TEntity).GetProperty(pascalCaseField);
                if (property == null)
                {
                    continue;
                }

                var member = Expression.Property(param, property);
                var lambda = Expression.Lambda<Func<TEntity, object>>(
                    Expression.Convert(member, typeof(object)),
                    param
                );

                if (i == 0 && order == OrderByTypes.Asc)
                {
                    query = query.OrderBy(lambda);
                }
                else if (i == 0 && order == OrderByTypes.Desc)
                {
                    query = query.OrderByDescending(lambda);
                }
                else if (order == OrderByTypes.Asc)
                {
                    query = ((IOrderedQueryable<TEntity>)query).ThenBy(lambda);
                }
                else
                {
                    query = ((IOrderedQueryable<TEntity>)query).ThenByDescending(lambda);
                }
            }
            return query;
        }

        /// <summary>
        /// kiểm tra nhiều điều kiện
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> WhenMany<TEntity>(
            this IQueryable<TEntity> query,
            IList<Expression<Func<TEntity, bool>>>? predicates
        )
            where TEntity : class
        {
            if (predicates == null)
            {
                return query;
            }

            for (var i = 0; i < predicates.Count; i++)
            {
                query.Where(predicates[i]);
            }
            return query;
        }
    }
}
