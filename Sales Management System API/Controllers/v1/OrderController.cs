using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.OrderDtos;
using Sales_Management_System_API.Services.Interfaces;
using System.Security.Claims;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderQueryDto query)
        {
            var userId = GetUserId();
            var result = await _orderService.GetByUserIdAsync(userId, query);
            return Ok(new ApiResponse<PagedResult<OrderDto>>
            {
                Success = true,
                Message = "Orders retrieved successfully",
                Data = result
            });
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderQueryDto query)
        {
            var result = await _orderService.GetAllAsync(query);
            return Ok(new ApiResponse<PagedResult<OrderDto>>
            {
                Success = true,
                Message = "Orders retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order retrieved successfully",
                Data = result
            });
        }

        [HttpGet("code/{orderCode}")]
        public async Task<IActionResult> GetByOrderCode(string orderCode)
        {
            var result = await _orderService.GetByOrderCodeAsync(orderCode);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _orderService.CreateFromCartAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.OrderId },
                new ApiResponse<OrderDto>
                {
                    Success = true,
                    Message = "Order created successfully",
                    Data = result
                });
        }

        [HttpPut("{id}/status")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.UpdateOrderStatusAsync(id, dto);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order status updated successfully",
                Data = result
            });
        }

        [HttpPut("{id}/payment-status")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.UpdatePaymentStatusAsync(id, dto);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Payment status updated successfully",
                Data = result
            });
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order cancelled successfully",
                Data = result
            });
        }
    }
}
