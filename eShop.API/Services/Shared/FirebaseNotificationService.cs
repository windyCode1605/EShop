using FirebaseAdmin.Messaging;

namespace eShop.API.Services.Shared;
public class FirebaseNotificationService
{
    // Gửi cho một user cụ thể ( dựa vào deviceToken của họ)
    public async Task<string> SendToDeviceAsync(string deviceToken, string title, string body)
    {
        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };
        // Hàm SendAsync sẽ gọi Firebase để bắn Noti đi. Trả về messageId nếu thành công.
        return await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }

    // Gửi thông báo cho hàng loạt người (vd: topic "all-users" hoặc "promotions")
    public async Task<string> SendToTopicAsync(string topic, string title, string body)
    {
        var message = new Message()
        {
            Topic = topic,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };
        return await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}