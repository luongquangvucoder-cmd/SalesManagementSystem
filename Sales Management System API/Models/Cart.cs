namespace Sales_Management_System_API.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
