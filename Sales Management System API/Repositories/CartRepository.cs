using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO.CartDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Cart Queries

        public async Task<CartDto?> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .ThenInclude(v => v.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart == null ? null : MapToCartDto(cart);
        }

        public async Task<CartDto?> GetCartByIdAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .ThenInclude(v => v.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            return cart == null ? null : MapToCartDto(cart);
        }

        public async Task<Cart?> GetCartEntityByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartEntityByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        #endregion

        #region Cart Operations

        public async Task<CartDto> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            var createdCart = await GetCartByIdAsync(cart.CartId);
            return createdCart!;
        }

        public async Task ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Cart Item Queries

        public async Task<CartItemDto?> GetCartItemByIdAsync(int cartItemId)
        {
            var item = await _context.CartItems
                .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            return item == null ? null : MapToCartItemDto(item);
        }

        public async Task<CartItemDto?> GetCartItemByVariantAsync(int cartId, int productVariantId)
        {
            var item = await _context.CartItems
                .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cartId &&
                    ci.ProductVariantId == productVariantId);

            return item == null ? null : MapToCartItemDto(item);
        }

        #endregion

        #region Cart Item Operations

        public async Task<CartItemDto> AddItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            var addedItem = await GetCartItemByIdAsync(cartItem.CartItemId);
            return addedItem!;
        }

        public async Task<CartItemDto> UpdateItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            var updatedItem = await GetCartItemByIdAsync(cartItem.CartItemId);
            return updatedItem!;
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Stock Check

        public async Task<int> GetVariantStockAsync(int productVariantId)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ProductVariantId == productVariantId);

            return variant?.StockQuantity ?? 0;
        }

        #endregion

        #region Helper Methods

        private static CartDto MapToCartDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items.Select(MapToCartItemDto).ToList()
            };
        }

        private static CartItemDto MapToCartItemDto(CartItem item)
        {
            return new CartItemDto
            {
                CartItemId = item.CartItemId,
                CartId = item.CartId,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant?.Product?.Name ?? string.Empty,
                SKU = item.ProductVariant?.SKU ?? string.Empty,
                Price = item.ProductVariant?.Price ?? 0,
                Quantity = item.Quantity,
                AvailableStock = item.ProductVariant?.StockQuantity ?? 0
            };
        }

        #endregion
    }
}
