namespace CR.Core.Dtos.DashboardModule
{
    /// <summary>
    /// Response trả về cho màn hình Admin Dashboard
    /// </summary>
    public class DashboardResponseDto
    {
        /// <summary>Thống kê tổng quan (4 thẻ con số)</summary>
        public DashboardSummaryDto Summary { get; set; } = new();

        /// <summary>Dữ liệu biểu đồ doanh thu theo tháng trong năm hiện tại</summary>
        public List<ChartPointDto> RevenueChart { get; set; } = new();

        /// <summary>Top 5 sản phẩm bán chạy nhất</summary>
        public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();
    }

    /// <summary>
    /// 4 thẻ thống kê ở đầu trang Dashboard
    /// </summary>
    public class DashboardSummaryDto
    {
        // Tháng hiện tại
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int NewCustomers { get; set; }
        public int PendingOrders { get; set; }

        // % tăng trưởng so với tháng trước (dương = tăng, âm = giảm)
        public double RevenueGrowthPercent { get; set; }
        public double OrderGrowthPercent { get; set; }
        public double CustomerGrowthPercent { get; set; }
    }

    /// <summary>
    /// Một điểm dữ liệu trên biểu đồ Revenue (1 điểm = 1 tháng)
    /// </summary>
    public class ChartPointDto
    {
        /// <summary>Nhãn trên trục X, ví dụ: "Jan", "Feb", "Tháng 1", ...</summary>
        public string Label { get; set; } = null!;

        /// <summary>Doanh thu của tháng đó</summary>
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// Thông tin một sản phẩm trong danh sách Top Selling
    /// </summary>
    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }

        /// <summary>Tổng số lượng đã bán</summary>
        public int TotalSold { get; set; }
    }
}
