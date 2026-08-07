using CR.ApplicationBase.Common;
using CR.Constants.Core.Users;
using CR.Constants.Orders;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.DashboardModule.Abstracts;
using CR.Core.Dtos.DashboardModule;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.DashboardModule.Implements;

public class DashboardService : CoreServiceBase, IDashboardService
{
    public DashboardService(ILogger<DashboardService> logger, IHttpContextAccessor httpContext)
        : base(logger, httpContext) { }

    public async Task<Result<DashboardResponseDto>> GetDashboardSummaryAsync()
    {
        _logger.LogInformation("Method : {method}", nameof(GetDashboardSummaryAsync));

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        var yearOrders = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= yearStart)
            .Select(o => new { o.Status, o.CreatedDate, o.TotalAmount })
            .ToListAsync();

        // Tính toán trong bộ nhớ (không thêm DB round-trip)
        var currentRevenue = yearOrders
            .Where(o => o.Status == OrderStatusConst.Delivered && o.CreatedDate >= currentMonthStart)
            .Sum(o => o.TotalAmount);
        var previousRevenue = yearOrders
            .Where(o => o.Status == OrderStatusConst.Delivered
                     && o.CreatedDate >= previousMonthStart
                     && o.CreatedDate < currentMonthStart)
            .Sum(o => o.TotalAmount);
        var currentOrders = yearOrders.Count(o => o.CreatedDate >= currentMonthStart);
        var previousOrders = yearOrders.Count(o => o.CreatedDate >= previousMonthStart
                                                 && o.CreatedDate < currentMonthStart);

        var pendingOrders = await _dbContext.Orders
            .AsNoTracking()
            .CountAsync(o => o.Status == OrderStatusConst.Pending);

        //  Khách hàng mới — 1 query cho cả 2 tháng
        var twoMonthCustomers = await _dbContext.Users
            .AsNoTracking()
            .Where(u => (int)u.UserType == UserType.CUSTOMER
                     && u.CreatedDate >= previousMonthStart
                     && !u.Deleted)
            .Select(u => new { u.CreatedDate })
            .ToListAsync();

        var currentCustomers = twoMonthCustomers.Count(u => u.CreatedDate >= currentMonthStart);
        var previousCustomers = twoMonthCustomers.Count(u => u.CreatedDate < currentMonthStart);

        //  Top 5 sản phẩm bán chạy nhất (All time) 
        var topSelling = await _dbContext.OrderItems
            .AsNoTracking()
            .GroupBy(oi => oi.ProductVariant.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(oi => oi.Quantity),
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .Join(
                _dbContext.Products.AsNoTracking().Where(p => !p.Deleted),
                t => t.ProductId,
                p => p.Id,
                (t, p) => new { t.ProductId, t.TotalSold, p.Name, p.BasePrice }
            )
            .ToListAsync();

        //  QUERY 5: Ảnh đại diện của 5 sản phẩm — 1 query, chỉ lấy 2 cột 
        var productIds = topSelling.Select(t => t.ProductId).ToList();
        var imageMap = (await _dbContext.ProductImages
            .AsNoTracking()
            .Where(i => productIds.Contains(i.ProductId) && i.IsPrimary && !i.Deleted)
            .Select(i => new { i.ProductId, i.Url })
            .ToListAsync())
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.First().Url); // GroupBy phòng trường hợp 1 product có >1 ảnh IsPrimary

        //  Tổng hợp kết quả 
        var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        var result = new DashboardResponseDto
        {
            Summary = new DashboardSummaryDto
            {
                TotalRevenue = currentRevenue,
                TotalOrders = currentOrders,
                NewCustomers = currentCustomers,
                PendingOrders = pendingOrders,
                RevenueGrowthPercent = CalcGrowth(previousRevenue, currentRevenue),
                OrderGrowthPercent = CalcGrowth(previousOrders, currentOrders),
                CustomerGrowthPercent = CalcGrowth(previousCustomers, currentCustomers),
            },

            // Biểu đồ: tính từ yearOrders đã load sẵn — không cần thêm DB query
            RevenueChart = Enumerable.Range(1, now.Month)
                .Select(m => new ChartPointDto
                {
                    Label = monthNames[m - 1],
                    Revenue = yearOrders
                        .Where(o => o.Status == OrderStatusConst.Delivered
                                 && o.CreatedDate!.Value.Month == m)
                        .Sum(o => o.TotalAmount),
                })
                .ToList(),

            TopSellingProducts = topSelling.Select(t => new TopSellingProductDto
            {
                ProductId = t.ProductId,
                ProductName = t.Name,
                Price = t.BasePrice,
                TotalSold = t.TotalSold,
                ImageUrl = imageMap.GetValueOrDefault(t.ProductId),
            }).ToList(),
        };

        return Result<DashboardResponseDto>.Success(result);
    }

    /// <summary>
    /// Tính % tăng trưởng so với kỳ trước.
    /// Trả về 0 nếu kỳ trước không có dữ liệu (tránh chia cho 0).
    /// Dương = tăng trưởng, Âm = sụt giảm.
    /// </summary>
    private static double CalcGrowth(decimal previous, decimal current)
    {
        if (previous == 0) return 0;
        return Math.Round((double)((current - previous) / previous * 100), 1);
    }

    private static double CalcGrowth(int previous, int current)
        => CalcGrowth((decimal)previous, (decimal)current);
}
