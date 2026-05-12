using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.OrderDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        #region Order Queries

        public async Task<PagedResult<OrderDto>> GetAllAsync(OrderQueryDto query)
        {
            if (query.Page < 1)
                throw new BadRequestException("Page must be greater than 0");

            if (query.PageSize < 1 || query.PageSize > 100)
                throw new BadRequestException("PageSize must be between 1 and 100");

            return await _orderRepository.GetAllAsync(query);
        }

        public async Task<PagedResult<OrderDto>> GetByUserIdAsync(string userId, OrderQueryDto query)
        {
            if (query.Page < 1)
                throw new BadRequestException("Page must be greater than 0");

            if (query.PageSize < 1 || query.PageSize > 100)
                throw new BadRequestException("PageSize must be between 1 and 100");

            return await _orderRepository.GetByUserIdAsync(userId, query);
        }

        public async Task<OrderDto> GetByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            return order;
        }

        public async Task<OrderDto> GetByOrderCodeAsync(string orderCode)
        {
            var order = await _orderRepository.GetByOrderCodeAsync(orderCode);
            if (order == null)
                throw new NotFoundException($"Order with code {orderCode} not found");

            return order;
        }

        #endregion

        #region Order Operations

        public async Task<OrderDto> CreateFromCartAsync(string userId, CreateOrderDto dto)
        {
            // Get user's cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new BadRequestException("Cart not found or is empty");

            if (cart.Items.Count == 0)
                throw new BadRequestException("Cart is empty");

            // Generate unique order code
            var orderCode = await _orderRepository.GenerateOrderCodeAsync();

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderCode = orderCode,
                TotalAmount = cart.TotalPrice,
                OrderStatus = OrderStatusConstants.Pending,
                PaymentStatus = PaymentStatusConstants.Unpaid,
                ShippingAddress = dto.ShippingAddress,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                CreatedAt = DateTime.UtcNow
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Create order items from cart
            var cartEntity = await _cartRepository.GetCartEntityByUserIdAsync(userId);
            if (cartEntity != null)
            {
                foreach (var cartItem in cartEntity.Items)
                {
                    var variant = await _productRepository.GetVariantByIdAsync(cartItem.ProductVariantId);
                    if (variant == null)
                        throw new BadRequestException($"Product variant with id {cartItem.ProductVariantId} not found");

                    var product = await _productRepository.GetProductByIdAsync(cartItem.ProductVariant?.ProductId ?? 0);
                    if (product == null)
                        throw new BadRequestException("Product not found");

                    var orderItem = new OrderItem
                    {
                        OrderId = createdOrder.OrderId,
                        ProductVariantId = cartItem.ProductVariantId,
                        ProductName = product.Name,
                        SKU = variant.SKU,
                        UnitPrice = variant.Price,
                        Quantity = cartItem.Quantity,
                        SubTotal = variant.Price * cartItem.Quantity
                    };

                    await _orderRepository.AddOrderItemAsync(orderItem);
                }

                // Clear cart
                await _cartRepository.ClearCartAsync(cartEntity.CartId);
            }

            return await _orderRepository.GetByIdAsync(createdOrder.OrderId)
                ?? throw new BadRequestException("Failed to retrieve created order");
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            // Validate order exists
            var order = await _orderRepository.GetOrderEntityByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            // Validate status
            var validStatuses = OrderStatusConstants.GetAllStatuses();
            if (!validStatuses.Contains(dto.OrderStatus))
                throw new BadRequestException($"Invalid order status. Valid statuses: {string.Join(", ", validStatuses)}");

            // Validate status transition
            if (order.OrderStatus == OrderStatusConstants.Cancelled)
                throw new BadRequestException("Cannot update status of a cancelled order");

            if (order.OrderStatus == OrderStatusConstants.Completed)
                throw new BadRequestException("Cannot update status of a completed order");

            order.OrderStatus = dto.OrderStatus;
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderDto> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusDto dto)
        {
            // Validate order exists
            var order = await _orderRepository.GetOrderEntityByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            // Validate payment status
            var validStatuses = PaymentStatusConstants.GetAllStatuses();
            if (!validStatuses.Contains(dto.PaymentStatus))
                throw new BadRequestException($"Invalid payment status. Valid statuses: {string.Join(", ", validStatuses)}");

            order.PaymentStatus = dto.PaymentStatus;
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderEntityByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            if (order.OrderStatus == OrderStatusConstants.Completed)
                throw new BadRequestException("Cannot cancel a completed order");

            if (order.OrderStatus == OrderStatusConstants.Cancelled)
                throw new BadRequestException("Order is already cancelled");

            order.OrderStatus = OrderStatusConstants.Cancelled;
            return await _orderRepository.UpdateAsync(order);
        }

        #endregion
    }
}
