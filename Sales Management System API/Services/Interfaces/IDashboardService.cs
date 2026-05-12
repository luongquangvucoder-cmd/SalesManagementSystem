using Sales_Management_System_API.DTO.DashboardDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IDashboardService
    {
        // Overall statistics
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<CategorySalesDto>> GetCategorySalesAsync();
        Task<PeriodStatsDto> GetPeriodStatsAsync(string period); // daily, weekly, monthly, yearly
    }
}
