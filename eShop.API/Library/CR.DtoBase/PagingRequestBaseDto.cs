// CR.DtoBase/PagingRequestBaseDto.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CR.DtoBase;

public class PagingRequestBaseDto
{
    // ── PAGE/PAGESIZE MODE ────────────────────────────────────────────────

    [FromQuery(Name = "pageSize")]
    [Range(1, 500, ErrorMessage = "pageSize phải từ 1 đến 500")]
    public int? PageSize { get; set; }

    [FromQuery(Name = "pageNumber")]
    [Range(1, int.MaxValue, ErrorMessage = "pageNumber phải >= 1")]
    public int? PageNumber { get; set; }

    // ── OFFSET/LIMIT MODE ─────────────────────────────────────────────────

    [FromQuery(Name = "limit")]
    [Range(1, 500, ErrorMessage = "limit phải từ 1 đến 500")]
    public int? Limit { get; set; }

    [FromQuery(Name = "offset")]
    [Range(0, int.MaxValue, ErrorMessage = "offset phải >= 0")]
    public int? Offset { get; set; }

    // ── KEYWORD ───────────────────────────────────────────────────────────

    private string? _keyword;

    [FromQuery(Name = "keyword")]
    public string? Keyword
    {
        get => _keyword;
        set => _keyword = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    // ── SORTING ───────────────────────────────────────────────────────────

    /// <summary>
    /// Danh sách field sort. Format: "fieldName:asc" hoặc "fieldName:desc"
    /// VD: ?sortBy=createdDate:desc&sortBy=name:asc
    /// </summary>
    [FromQuery(Name = "sortBy")]
    public List<string> Sort { get; set; } = [];

    // ── HELPERS ───────────────────────────────────────────────────────────

    public bool IsPagingByPage()   => PageSize.HasValue && PageNumber.HasValue;
    public bool IsPagingByOffset() => Limit.HasValue && Offset.HasValue;
    public bool HasPaging()        => IsPagingByPage() || IsPagingByOffset();

    public int GetSkip()
    {
        if (IsPagingByPage())
            return (PageNumber!.Value - 1) * PageSize!.Value;

        if (IsPagingByOffset())
            return Offset!.Value;

        return 0;
    }

    public int GetTake()
    {
        if (IsPagingByPage())
            return PageSize == -1 ? int.MaxValue : PageSize!.Value;

        if (IsPagingByOffset())
            return Limit!.Value;

        return 0; // 0 = không giới hạn
    }

    /// <summary>
    /// Validate: phải truyền đủ cặp (pageNumber + pageSize) hoặc (offset + limit),
    /// không được truyền lẫn lộn.
    /// </summary>
    public (bool IsValid, string? Error) Validate()
    {
        var hasPage   = PageSize.HasValue || PageNumber.HasValue;
        var hasOffset = Limit.HasValue || Offset.HasValue;

        if (hasPage && hasOffset)
            return (false, "Không được dùng đồng thời pageSize/pageNumber và limit/offset.");

        if (PageNumber.HasValue && !PageSize.HasValue)
            return (false, "Cần truyền pageSize khi dùng pageNumber.");

        if (PageSize.HasValue && !PageNumber.HasValue)
            return (false, "Cần truyền pageNumber khi dùng pageSize.");

        if (Offset.HasValue && !Limit.HasValue)
            return (false, "Cần truyền limit khi dùng offset.");

        if (Limit.HasValue && !Offset.HasValue)
            return (false, "Cần truyền offset khi dùng limit.");

        return (true, null);
    }
}