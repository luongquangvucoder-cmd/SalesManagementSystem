using Sales_Management_System_API.DTO.CartDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface ICartService
    {
        // Cart operations
        Task<CartDto> GetOrCreateCartAsync(string userId);
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveFromCartAsync(string userId, int cartItemId);
        Task<CartDto> ClearCartAsync(string userId);
    }
}
