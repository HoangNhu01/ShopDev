using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace ShopDev.ApplicationBase.Common
{
    public class PagingRequestBaseGenericDto<TDto>
    {
        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; }

        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; set; }

        private string? _keyword { get; set; }

        [FromQuery(Name = "keyword")]
        public string? Keyword
        {
            get => _keyword;
            set => _keyword = value?.Trim();
        }

        public int GetSkip()
        {
            int skip = (PageNumber - 1) * PageSize;
            if (skip < 0)
            {
                skip = 0;
            }
            return skip;
        }

        public List<string> Sort { get; set; } = new();

        public virtual List<Expression<Func<TDto, bool>>> GetPredicates()
        {
            return [];
        }
    }
}
