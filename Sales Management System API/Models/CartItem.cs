namespace Sales_Management_System_API.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public int CartId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public Cart Cart { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
