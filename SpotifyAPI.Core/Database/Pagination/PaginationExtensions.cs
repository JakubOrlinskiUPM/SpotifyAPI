using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace SpotifyAPI.Core.Database.Pagination;

public static class PaginationExtensions
{
    
    public static async Task<Paginated<T>> Paginate<T> (this IQueryable<T> queryable, QueryModel queryModel)
        where T : class
    {
        var filtered = queryable.ApplyFilter(queryModel);
        var count = await filtered.CountAsync();

        var data = await filtered.ApplyPagination(queryModel)
            .ToListAsync();

        return new Paginated<T>
        {
            Data = data,
            Limit = queryModel.Limit,
            Page = queryModel.Page,
            TotalCount = count,
            Order = queryModel.Order,
            Filter = queryModel.Filter,
        };
    }
    
    public static async Task<Paginated<TResult>> PaginateAs<T, TResult> (
        this IQueryable<T> queryable,
        QueryModel queryModel
    ) where T : class where TResult : class
    {
        var filtered = queryable.ApplyFilter(queryModel);
        var count = await filtered.CountAsync();

        var data = await filtered.ApplyPagination(queryModel)
            .ProjectToType<TResult>()
            .ToListAsync();

        return new Paginated<TResult>
        {
            Data = data,
            Limit = queryModel.Limit,
            Page = queryModel.Page,
            TotalCount = count,
            Order = queryModel.Order,
            Filter = queryModel.Filter,
        };
    }
    
    
    public static IQueryable<T> ApplyPagination<T> (
        this IQueryable<T> queryable,
        QueryModel queryModel
    ) where T : class
    {
        return queryable;
        // .OrderBy(queryModel.Order)
        // .Skip(queryModel.Limit * (queryModel.Page - 1))
        // .Take(queryModel.Limit);
    }

    public static IQueryable<T> ApplyFilter<T> (
        this IQueryable<T> queryable,
        QueryModel queryModel
    ) where T : class
    {
        return queryable;
        // return string.IsNullOrWhiteSpace(queryModel.Filter)
            // ? queryable
            // : queryable.Where(queryModel.Filter);
    }
}