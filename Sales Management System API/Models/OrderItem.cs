namespace Sales_Management_System_API.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public Order Order { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
