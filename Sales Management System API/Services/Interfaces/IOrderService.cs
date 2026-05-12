using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.OrderDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IOrderService
    {
        // Order queries
        Task<PagedResult<OrderDto>> GetAllAsync(OrderQueryDto query);
        Task<PagedResult<OrderDto>> GetByUserIdAsync(string userId, OrderQueryDto query);
        Task<OrderDto> GetByIdAsync(int orderId);
        Task<OrderDto> GetByOrderCodeAsync(string orderCode);

        // Order operations
        Task<OrderDto> CreateFromCartAsync(string userId, CreateOrderDto dto);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<OrderDto> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusDto dto);
        Task<OrderDto> CancelOrderAsync(int orderId);
    }
}
