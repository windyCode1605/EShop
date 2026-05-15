// CR.DtoBase/PageResult.cs
namespace CR.DtoBase;

public class PageResult<T>
{
    public IEnumerable<T> Items      { get; set; } = Enumerable.Empty<T>();
    public long           TotalItems { get; set; }

    /// <summary>Tổng số trang (chỉ có giá trị khi paging theo Page/PageSize).</summary>
    public int? TotalPages { get; set; }

    /// <summary>Trang hiện tại.</summary>
    public int? CurrentPage { get; set; }

    /// <summary>Số item mỗi trang.</summary>
    public int? PageSize { get; set; }

    /// <summary>Còn trang tiếp theo không.</summary>
    public bool HasNextPage => CurrentPage.HasValue && TotalPages.HasValue
        && CurrentPage < TotalPages;

    /// <summary>Còn trang trước không.</summary>
    public bool HasPreviousPage => CurrentPage.HasValue && CurrentPage > 1;

    // ── FACTORY METHODS ──────────────────────────────────────────────────

    /// <summary>Tạo PageResult khi paging theo Page/PageSize.</summary>
    public static PageResult<T> Create(
        IEnumerable<T>       items,
        long                 totalItems,
        PagingRequestBaseDto input)
    {
        var result = new PageResult<T>
        {
            Items      = items,
            TotalItems = totalItems,
        };

        if (input.IsPagingByPage() && input.PageSize > 0)
        {
            result.CurrentPage = input.PageNumber;
            result.PageSize    = input.PageSize;
            result.TotalPages  = (int)Math.Ceiling(
                (double)totalItems / input.PageSize!.Value);
        }

        return result;
    }

    /// <summary>Tạo PageResult đơn giản không có metadata trang.</summary>
    public static PageResult<T> Create(IEnumerable<T> items, long totalItems)
        => new() { Items = items, TotalItems = totalItems };
}