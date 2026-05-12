namespace Sales_Management_System_API.DTO.DashboardDtos
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal UnpaidRevenue { get; set; }
        public int TotalUsers { get; set; }
        public List<OrderStatusCountDto> OrderStatusCounts { get; set; } = [];
        public List<PaymentStatusCountDto> PaymentStatusCounts { get; set; } = [];
        public List<TopProductDto> TopProducts { get; set; } = [];
        public List<MonthlySalesDto> MonthlySales { get; set; } = [];
    }

    public class OrderStatusCountDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class PaymentStatusCountDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlySalesDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }

    // Period Statistics
    public class PeriodStatsDto
    {
        public string Period { get; set; } = string.Empty; // "daily", "weekly", "monthly", "yearly"
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int NewUsers { get; set; }
    }

    // Category Sales
    public class CategorySalesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}
