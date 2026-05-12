using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CartDtos;
using Sales_Management_System_API.Services.Interfaces;
using System.Security.Claims;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetOrCreateCartAsync(userId);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Cart retrieved successfully",
                Data = result
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _cartService.AddToCartAsync(userId, dto);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Item added to cart successfully",
                Data = result
            });
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Cart item updated successfully",
                Data = result
            });
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Item removed from cart successfully",
                Data = result
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Cart cleared successfully",
                Data = result
            });
        }
    }
}
