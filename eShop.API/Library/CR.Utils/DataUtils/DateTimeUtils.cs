namespace CR.Utils.DataUtils
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// GetDate: Hàm này dùng để lấy ngày giờ hiện tại theo múi giờ của hệ thống, đã được chuẩn hóa về UTC để tránh lỗi liên quan đến múi giờ khi lưu trữ và xử lý dữ liệu.
        /// </summary>
        /// <returns>Ngày giờ hiện tại theo UTC</returns>
        public static DateTime GetDate()
        {
            return new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
        }
    }
}