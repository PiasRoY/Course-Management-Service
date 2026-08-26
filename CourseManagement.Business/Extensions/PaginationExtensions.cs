using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseManagement.Business.Extensions;

public static class PaginationExtensions
{
    public async static Task<PageResult<TOut>> GetItems<T, TOut, TKey>(
        this IQueryable<T> queryable,
        PaginationParams @params,
        Expression<Func<T, TOut>> projection,
        Expression<Func<T, TKey>> orderBy,
        CancellationToken cancellationToken)
    {
        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
                            .OrderBy(orderBy)
                            .Skip((@params.PageNumber - 1) * @params.PageSize)
                            .Take(@params.PageSize)
                            .Select(projection)
                            .ToListAsync(cancellationToken);

        return new PageResult<TOut>
        {
            Data = items,
            CurrentPageNumber = @params.PageNumber,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (@params.PageSize * 1.00))
        };
    }
}
