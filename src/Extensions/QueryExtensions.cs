using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VotRomania.Models;

namespace VotRomania.Extensions
{
    public static class QueryExtensions
    {
        // NB! Don’t change IQueryable to IEnumerable because otherwise regular Count() method of LINQ is called instead of Entity Framework one.
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query,
            int? page, int? pageSize) where T : class
        {
            var resultRowCount = await query.CountAsync();
            if (page == null || pageSize == null)
            {
                var singlePagedResult = new PagedResult<T>();
                singlePagedResult.CurrentPage = 1;
                singlePagedResult.PageSize = resultRowCount;
                singlePagedResult.RowCount = resultRowCount;
                singlePagedResult.Results = await query.ToListAsync();

                return singlePagedResult;
            }

            var result = new PagedResult<T>();
            result.CurrentPage = page.Value;
            result.PageSize = pageSize.Value;
            result.RowCount = resultRowCount;


            var pageCount = (double)result.RowCount / pageSize.Value;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page.Value - 1) * pageSize.Value;
            result.Results = await query.Skip(skip).Take(pageSize.Value).ToListAsync();

            return result;
        }

        public static IQueryable<T> ConditionalWhere<T>(
            this IQueryable<T> source,
            Func<bool> condition,
            Expression<Func<T, bool>> predicate)
        {
            if (condition())
            {
                return source.Where(predicate);
            }

            return source;
        }
    }
}