using CR.DtoBase.Notification;

/// <summary>
/// Interface dành cho các job tạo thông báo, sẽ được đăng ký trong DI container và gọi bởi NotificationService
/// </summary>
namespace CR.InfrastructureBase.Notification
{
    public interface CreateJob
    {
        Task CreateNotiJob<INotiJob, TData>(INotiJobHandler job)
            where INotiJob : NotiJob<TData>
            where TData : class;
    }
}