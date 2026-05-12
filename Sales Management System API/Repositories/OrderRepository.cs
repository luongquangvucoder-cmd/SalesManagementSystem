using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.OrderDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Order Queries

        public async Task<PagedResult<OrderDto>> GetAllAsync(OrderQueryDto query)
        {
            var queryable = _context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .AsNoTracking();

            // Filter by order code
            if (!string.IsNullOrWhiteSpace(query.OrderCode))
            {
                queryable = queryable.Where(o => o.OrderCode.Contains(query.OrderCode));
            }

            // Filter by order status
            if (!string.IsNullOrWhiteSpace(query.OrderStatus))
            {
                queryable = queryable.Where(o => o.OrderStatus == query.OrderStatus);
            }

            // Filter by payment status
            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                queryable = queryable.Where(o => o.PaymentStatus == query.PaymentStatus);
            }

            // Filter by date range
            if (query.FromDate.HasValue)
            {
                queryable = queryable.Where(o => o.CreatedAt >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                queryable = queryable.Where(o => o.CreatedAt <= query.ToDate.Value.AddDays(1));
            }

            // Count total
            var totalCount = await queryable.CountAsync();

            // Sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "orderstatus" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.OrderStatus)
                    : queryable.OrderBy(o => o.OrderStatus),
                "totalamount" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.TotalAmount)
                    : queryable.OrderBy(o => o.TotalAmount),
                "ordercode" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.OrderCode)
                    : queryable.OrderBy(o => o.OrderCode),
                _ => query.IsDescending
                    ? queryable.OrderByDescending(o => o.CreatedAt)
                    : queryable.OrderBy(o => o.CreatedAt)
            };

            // Pagination
            var skip = (query.Page - 1) * query.PageSize;
            var orders = await queryable
                .Skip(skip)
                .Take(query.PageSize)
                .Select(o => MapToOrderDto(o))
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<PagedResult<OrderDto>> GetByUserIdAsync(string userId, OrderQueryDto query)
        {
            var queryable = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .AsNoTracking();

            // Filter by order code
            if (!string.IsNullOrWhiteSpace(query.OrderCode))
            {
                queryable = queryable.Where(o => o.OrderCode.Contains(query.OrderCode));
            }

            // Filter by order status
            if (!string.IsNullOrWhiteSpace(query.OrderStatus))
            {
                queryable = queryable.Where(o => o.OrderStatus == query.OrderStatus);
            }

            // Filter by payment status
            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                queryable = queryable.Where(o => o.PaymentStatus == query.PaymentStatus);
            }

            // Filter by date range
            if (query.FromDate.HasValue)
            {
                queryable = queryable.Where(o => o.CreatedAt >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                queryable = queryable.Where(o => o.CreatedAt <= query.ToDate.Value.AddDays(1));
            }

            // Count total
            var totalCount = await queryable.CountAsync();

            // Sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "orderstatus" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.OrderStatus)
                    : queryable.OrderBy(o => o.OrderStatus),
                "totalamount" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.TotalAmount)
                    : queryable.OrderBy(o => o.TotalAmount),
                "ordercode" => query.IsDescending
                    ? queryable.OrderByDescending(o => o.OrderCode)
                    : queryable.OrderBy(o => o.OrderCode),
                _ => query.IsDescending
                    ? queryable.OrderByDescending(o => o.CreatedAt)
                    : queryable.OrderBy(o => o.CreatedAt)
            };

            // Pagination
            var skip = (query.Page - 1) * query.PageSize;
            var orders = await queryable
                .Skip(skip)
                .Take(query.PageSize)
                .Select(o => MapToOrderDto(o))
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<OrderDto?> GetByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            return order == null ? null : MapToOrderDto(order);
        }

        public async Task<Order?> GetOrderEntityByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<OrderDto?> GetByOrderCodeAsync(string orderCode)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            return order == null ? null : MapToOrderDto(order);
        }

        public async Task<bool> IsOrderCodeUniqueAsync(string orderCode)
        {
            return !await _context.Orders
                .AnyAsync(o => o.OrderCode == orderCode);
        }

        #endregion

        #region Order Operations

        public async Task<OrderDto> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var createdOrder = await GetByIdAsync(order.OrderId);
            return createdOrder!;
        }

        public async Task<OrderDto> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            var updatedOrder = await GetByIdAsync(order.OrderId);
            return updatedOrder!;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePaymentStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = status;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Order Item Operations

        public async Task AddOrderItemAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion

        #region Generate Order Code

        public async Task<string> GenerateOrderCodeAsync()
        {
            string orderCode;
            bool isUnique = false;

            do
            {
                orderCode = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";
                isUnique = await IsOrderCodeUniqueAsync(orderCode);
            } while (!isUnique);

            return orderCode;
        }

        #endregion

        #region Helper Methods

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                ShippingAddress = order.ShippingAddress,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductName,
                    SKU = oi.SKU,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    SubTotal = oi.SubTotal
                }).ToList()
            };
        }

        #endregion
    }
}
