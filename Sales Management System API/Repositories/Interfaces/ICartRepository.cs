using Sales_Management_System_API.DTO.CartDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface ICartRepository
    {
        // Cart queries
        Task<CartDto?> GetCartByUserIdAsync(string userId);
        Task<CartDto?> GetCartByIdAsync(int cartId);
        Task<Cart?> GetCartEntityByUserIdAsync(string userId);
        Task<Cart?> GetCartEntityByIdAsync(int cartId);

        // Cart operations
        Task<CartDto> CreateAsync(Cart cart);
        Task ClearCartAsync(int cartId);

        // Cart item queries
        Task<CartItemDto?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItemDto?> GetCartItemByVariantAsync(int cartId, int productVariantId);

        // Cart item operations
        Task<CartItemDto> AddItemAsync(CartItem cartItem);
        Task<CartItemDto> UpdateItemAsync(CartItem cartItem);
        Task RemoveItemAsync(int cartItemId);

        // Check stock
        Task<int> GetVariantStockAsync(int productVariantId);
    }
}
