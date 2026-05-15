// CR.InfrastructureBase/Extensions/QueryableExtensions.cs
using System.Linq.Expressions;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.InfrastructureBase.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Apply paging lên IQueryable dựa trên PagingRequestBaseDto.
    /// Hỗ trợ 2 mode:
    ///   - Page/PageSize: truyền pageNumber + pageSize
    ///   - Offset/Limit:  truyền offset + limit
    /// Nếu không truyền gì → trả về toàn bộ (không giới hạn).
    /// </summary>
    public static IQueryable<T> Paging<T>(
        this IQueryable<T>    query,
        PagingRequestBaseDto  input)
    {
        var skip = input.GetSkip();
        var take = input.GetTake();

        if (skip > 0)
            query = query.Skip(skip);

        // take = 0 nghĩa là không phân trang → trả hết
        if (take > 0)
            query = query.Take(take);

        return query;
    }

    /// <summary>
    /// Apply sorting động theo danh sách field name từ query string.
    /// VD: sortBy=name-asc&sortBy=createdDate-desc
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T>    query,
        List<string>          sortFields,
        // Default sort khi không truyền sortBy
        Expression<Func<T, object>>? defaultSort = null,
        bool defaultDescending = true)
    {
        if (sortFields == null || !sortFields.Any())
        {
            // Áp dụng default sort nếu có
            if (defaultSort != null)
                return defaultDescending
                    ? query.OrderByDescending(defaultSort)
                    : query.OrderBy(defaultSort);

            return query;
        }

        IOrderedQueryable<T>? ordered = null;

        foreach (var field in sortFields)
        {
            // Format: "fieldName:asc" hoặc "fieldName:desc"
            var parts     = field.Split('-');
            var fieldName = parts[0].Trim();
            var direction = parts.Length > 1
                ? parts[1].Trim().ToLower()
                : "asc";

            var parameter  = Expression.Parameter(typeof(T), "x");
            var property   = Expression.Property(parameter, fieldName);
            var lambda     = Expression.Lambda<Func<T, object>>(
                Expression.Convert(property, typeof(object)), parameter);

            ordered = ordered == null
                ? direction == "desc"
                    ? query.OrderByDescending(lambda)
                    : query.OrderBy(lambda)
                : direction == "desc"
                    ? ordered.ThenByDescending(lambda)
                    : ordered.ThenBy(lambda);
        }

        return ordered ?? query;
    }

    /// <summary>
    /// Kết hợp paging + sorting + đếm tổng trong 1 lần gọi.
    /// Tránh viết lặp CountAsync() + ToListAsync() ở mọi service.
    /// </summary>
    public static async Task<PageResult<T>> ToPageResultAsync<T>(
        this IQueryable<T>   query,
        PagingRequestBaseDto input,
        CancellationToken    cancellationToken = default)
    {
        // Đếm tổng TRƯỚC khi apply paging
        var total = await query.LongCountAsync(cancellationToken);

        // Apply paging
        var items = await query
            .Paging(input)
            .ToListAsync(cancellationToken);

        return new PageResult<T>
        {
            TotalItems = total,
            Items      = items,
        };
    }
}