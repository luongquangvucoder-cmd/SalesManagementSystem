using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.DashboardDtos;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _dashboardService.GetDashboardStatsAsync();
            return Ok(new ApiResponse<DashboardStatsDto>
            {
                Success = true,
                Message = "Dashboard statistics retrieved successfully",
                Data = result
            });
        }

        [HttpGet("category-sales")]
        public async Task<IActionResult> GetCategorySales()
        {
            var result = await _dashboardService.GetCategorySalesAsync();
            return Ok(new ApiResponse<List<CategorySalesDto>>
            {
                Success = true,
                Message = "Category sales data retrieved successfully",
                Data = result
            });
        }

        [HttpGet("period-stats")]
        public async Task<IActionResult> GetPeriodStats([FromQuery] string period = "monthly")
        {
            var result = await _dashboardService.GetPeriodStatsAsync(period);
            return Ok(new ApiResponse<PeriodStatsDto>
            {
                Success = true,
                Message = "Period statistics retrieved successfully",
                Data = result
            });
        }
    }
}
