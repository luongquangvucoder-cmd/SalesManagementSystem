using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.CartDtos
{
    // Tr? v? client
    public class CartDto
    {
        public int CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<CartItemDto> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    // Cart Item
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int AvailableStock { get; set; }
    }

    // Add to cart
    public class AddToCartDto
    {
        [Required(ErrorMessage = "ProductVariantId is required")]
        public int ProductVariantId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    // Update cart item
    public class UpdateCartItemDto
    {
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
