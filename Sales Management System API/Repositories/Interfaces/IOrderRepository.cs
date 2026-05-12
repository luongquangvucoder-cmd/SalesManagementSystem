using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.OrderDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // Order queries
        Task<PagedResult<OrderDto>> GetAllAsync(OrderQueryDto query);
        Task<PagedResult<OrderDto>> GetByUserIdAsync(string userId, OrderQueryDto query);
        Task<OrderDto?> GetByIdAsync(int orderId);
        Task<Order?> GetOrderEntityByIdAsync(int orderId);
        Task<OrderDto?> GetByOrderCodeAsync(string orderCode);
        Task<bool> IsOrderCodeUniqueAsync(string orderCode);

        // Order operations
        Task<OrderDto> CreateAsync(Order order);
        Task<OrderDto> UpdateAsync(Order order);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task UpdatePaymentStatusAsync(int orderId, string status);

        // Order Item operations
        Task AddOrderItemAsync(OrderItem item);
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);

        // Generate order code
        Task<string> GenerateOrderCodeAsync();
    }
}
