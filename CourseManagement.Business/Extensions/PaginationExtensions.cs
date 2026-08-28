using CourseManagement.Business.DTOs.PaginationDTOs;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Business.Extensions;

public static class PaginationExtensions
{
    public async static Task<PageResult<T>> GetItemsAsync<T>(
        this IQueryable<T> queryable,
        PaginationParams @params,
        CancellationToken cancellationToken)
    {
        if (@params.PageNumber < 1 || @params.PageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be positive.");
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return new PageResult<T>
            {
                Data = [],
                CurrentPageNumber = @params.PageNumber,
                TotalCount = 0,
                TotalPages = 0
            };
        }

        var items = await queryable
                            .Skip((@params.PageNumber - 1) * @params.PageSize)
                            .Take(@params.PageSize)
                            .ToListAsync(cancellationToken);

        return new PageResult<T>
        {
            Data = items,
            CurrentPageNumber = @params.PageNumber,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (@params.PageSize * 1.00))
        };
    }
}
