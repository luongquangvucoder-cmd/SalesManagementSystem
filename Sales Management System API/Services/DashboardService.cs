using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO.DashboardDtos;
using Sales_Management_System_API.DTO.OrderDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            // Get basic counts
            var totalProducts = await _context.Products.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            // Get revenue data
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
            var unpaidRevenue = await _context.Orders
                .Where(o => o.PaymentStatus != "Paid")
                .SumAsync(o => o.TotalAmount);

            // Get order status counts
            var pendingOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == OrderStatusConstants.Pending);
            var completedOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == OrderStatusConstants.Completed);
            var cancelledOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == OrderStatusConstants.Cancelled);

            // Get status breakdowns
            var orderStatusCounts = await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new OrderStatusCountDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var paymentStatusCounts = await _context.Orders
                .GroupBy(o => o.PaymentStatus)
                .Select(g => new PaymentStatusCountDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Get top products
            var topProducts = await _context.OrderItems
                .GroupBy(oi => new { oi.ProductName, oi.ProductVariantId })
                .Select(g => new
                {
                    g.Key.ProductName,
                    ProductVariantId = g.Key.ProductVariantId,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.SubTotal)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            var topProductsList = topProducts.Select((p, idx) => new TopProductDto
            {
                ProductId = idx,
                ProductName = p.ProductName,
                TotalSold = p.TotalSold,
                TotalRevenue = p.TotalRevenue
            }).ToList();

            // Get monthly sales (last 12 months)
            var currentDate = DateTime.UtcNow;
            var monthlyData = new List<MonthlySalesDto>();

            for (int i = 11; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var startOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

                var monthlyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonth && o.CreatedAt <= endOfMonth)
                    .SumAsync(o => o.TotalAmount);

                var monthlyOrders = await _context.Orders
                    .CountAsync(o => o.CreatedAt >= startOfMonth && o.CreatedAt <= endOfMonth);

                monthlyData.Add(new MonthlySalesDto
                {
                    Month = date.Month,
                    Year = date.Year,
                    TotalSales = monthlyRevenue,
                    TotalOrders = monthlyOrders
                });
            }

            return new DashboardStatsDto
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                UnpaidRevenue = unpaidRevenue,
                TotalUsers = totalUsers,
                OrderStatusCounts = orderStatusCounts,
                PaymentStatusCounts = paymentStatusCounts,
                TopProducts = topProductsList,
                MonthlySales = monthlyData
            };
        }

        public async Task<List<CategorySalesDto>> GetCategorySalesAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .ThenInclude(p => p.Variants)
                .Select(c => new CategorySalesDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.Name,
                    TotalProducts = c.Products.Count,
                    TotalRevenue = c.Products
                        .SelectMany(p => p.Variants)
                        .Select(v => v.ProductVariantId)
                        .Join(_context.OrderItems.Where(oi => true), vid => vid, oi => oi.ProductVariantId, (vid, oi) => oi.SubTotal)
                        .Sum(),
                    TotalOrders = c.Products
                        .SelectMany(p => p.Variants)
                        .Select(v => v.ProductVariantId)
                        .Join(_context.OrderItems.Where(oi => true), vid => vid, oi => oi.ProductVariantId, (vid, oi) => oi.OrderId)
                        .Distinct()
                        .Count()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PeriodStatsDto> GetPeriodStatsAsync(string period)
        {
            var currentDate = DateTime.UtcNow;
            DateTime startDate;
            DateTime endDate = currentDate.AddSeconds(1); // Include today

            switch (period.ToLower())
            {
                case "daily":
                    startDate = currentDate.AddDays(-1);
                    break;
                case "weekly":
                    startDate = currentDate.AddDays(-7);
                    break;
                case "monthly":
                    startDate = currentDate.AddMonths(-1);
                    break;
                case "yearly":
                    startDate = currentDate.AddYears(-1);
                    break;
                default:
                    throw new BadRequestException("Invalid period. Valid periods: daily, weekly, monthly, yearly");
            }

            var periodOrders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();

            var totalRevenue = periodOrders.Sum(o => o.TotalAmount);
            var totalOrders = periodOrders.Count;

            var newUsers = await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .CountAsync();

            var newProducts = await _context.Products
                .CountAsync(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);

            return new PeriodStatsDto
            {
                Period = period.ToLower(),
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProducts = newProducts,
                NewUsers = newUsers
            };
        }
    }
}
