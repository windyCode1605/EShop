using CR.DtoBase.Interfaces;
using Microsoft.AspNetCore.Http.Features;

namespace CR.DtoBase.Notification;

/// <summary>
/// Khởi tạo việc làm thông báo
/// </summary>
public class NotiJob<TData> : IRequestId where TData : class
{
    /// <summary>
    /// Id của người thuê (tenant), nếu có, dùng để phân biệt dữ liệu trong trường hợp hệ thống hỗ trợ đa tenant
    /// </summary>
    public int? TenantId { get; set; }
    
    /// <summary>
    /// Id của khóa học, nếu có, dùng để phân biệt dữ liệu trong trường hợp hệ thống hỗ trợ nhiều khóa học
    /// </summary>

    public int? CourseId { get; set; }
    
    /// <summary>
    /// Key định danh sự kiện, dùng để xác định loại thông báo cần tạo, ví dụ: "NewOrder", "CourseCompleted", "UserRegistered", v.v.
    /// </summary>
    public string EventKey { get; set; } = null!;

    /// <summary>
    /// UserId nhân thông báo
    /// </summary>
    public IEnumerable<int> RecipientIds { get; set; } = [];
    /// <summary>
    /// Dữ liệu bổ sung cần thiết để tạo thông báo, có thể là thông tin về đơn hàng, khóa học, người dùng, v.v. Tùy thuộc vào loại thông báo mà EventKey xác định
    /// </summary>
    public TData? Data { get; set; }

    public string? RequestId { get; set; }

    public NotiJob()
    {
        
    }
    // Constructor để dễ dàng chuyển đổi từ NotiJob<TData> sang NotiJob (nếu cần)
    public NotiJob(NotiJob<TData> noti)
    {
        TenantId = noti.TenantId;
        CourseId = noti.CourseId;
        EventKey = noti.EventKey;
        RecipientIds = noti.RecipientIds;
        Data = noti.Data;
        RequestId = noti.RequestId;
    }
}

