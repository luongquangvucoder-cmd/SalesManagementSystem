using Sales_Management_System_API.DTO.CartDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDto> GetOrCreateCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart != null)
                return cart;

            // Create new cart
            var newCart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            return await _cartRepository.CreateAsync(newCart);
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                throw new NotFoundException($"Cart not found for user {userId}");

            return cart;
        }

        public async Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            // Validate product variant exists
            var variant = await _productRepository.GetVariantByIdAsync(dto.ProductVariantId);
            if (variant == null)
                throw new BadRequestException($"Product variant with id {dto.ProductVariantId} not found");

            // Check stock
            var stock = await _cartRepository.GetVariantStockAsync(dto.ProductVariantId);
            if (stock < dto.Quantity)
                throw new BadRequestException($"Insufficient stock. Available: {stock}, Requested: {dto.Quantity}");

            // Get or create cart
            var cart = await _cartRepository.GetCartEntityByUserIdAsync(userId);
            if (cart == null)
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _cartRepository.CreateAsync(newCart);

                // Retrieve the newly created cart
                cart = await _cartRepository.GetCartEntityByUserIdAsync(userId)
                    ?? throw new BadRequestException("Failed to create cart");
            }

            // Check if item already exists
            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductVariantId == dto.ProductVariantId);

            if (existingItem != null)
            {
                // Update quantity
                var newQuantity = existingItem.Quantity + dto.Quantity;

                if (stock < newQuantity)
                    throw new BadRequestException($"Insufficient stock. Available: {stock}, Requested: {newQuantity}");

                existingItem.Quantity = newQuantity;
                await _cartRepository.UpdateItemAsync(existingItem);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity
                };

                await _cartRepository.AddItemAsync(cartItem);
            }

            return await _cartRepository.GetCartByUserIdAsync(userId)
                ?? throw new BadRequestException("Failed to retrieve cart");
        }

        public async Task<CartDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetCartEntityByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException($"Cart not found for user {userId}");

            var cartItem = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (cartItem == null)
                throw new NotFoundException($"Cart item with id {cartItemId} not found");

            // Check stock
            var stock = await _cartRepository.GetVariantStockAsync(cartItem.ProductVariantId);
            if (stock < dto.Quantity)
                throw new BadRequestException($"Insufficient stock. Available: {stock}, Requested: {dto.Quantity}");

            cartItem.Quantity = dto.Quantity;
            await _cartRepository.UpdateItemAsync(cartItem);

            return await _cartRepository.GetCartByUserIdAsync(userId)
                ?? throw new BadRequestException("Failed to retrieve cart");
        }

        public async Task<CartDto> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cart = await _cartRepository.GetCartEntityByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException($"Cart not found for user {userId}");

            var cartItem = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (cartItem == null)
                throw new NotFoundException($"Cart item with id {cartItemId} not found");

            await _cartRepository.RemoveItemAsync(cartItemId);

            return await _cartRepository.GetCartByUserIdAsync(userId)
                ?? throw new BadRequestException("Failed to retrieve cart");
        }

        public async Task<CartDto> ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartEntityByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException($"Cart not found for user {userId}");

            await _cartRepository.ClearCartAsync(cart.CartId);

            return await _cartRepository.GetCartByUserIdAsync(userId)
                ?? throw new BadRequestException("Failed to retrieve cart");
        }
    }
}
