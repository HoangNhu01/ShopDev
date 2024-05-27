using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.Authentication.Infrastructure.Persistence;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public abstract class InventoryServiceBase : ServiceBase<InventoryDbContext>
    {
        protected InventoryServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

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
            where TDto : class
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
    }
}
