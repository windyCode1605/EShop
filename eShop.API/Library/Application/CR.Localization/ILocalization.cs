namespace CR.ApplicationBase.Localization
{

    public interface ILocalization
    {
        /// <summary>
        /// file này sẽ dùng để lưu các key và value ngôn ngữ khác nhau để 
        /// có thể dễ dàng thay đổi ngôn ngữ của ứng dụng mà không cần phải thay đổi code
        /// ví dụ: nếu muốn thay đổi ngôn ngữ của ứng dụng sang tiếng Việt, ta chỉ cần thay đổi giá trị của key "Hello" thành "Xin chào" trong file này
        /// </summary>
        string Localize(string keyName);
        string Localize(string keyName, string[]? listParams);

    }
};