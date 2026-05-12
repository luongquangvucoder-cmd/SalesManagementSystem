namespace Sales_Management_System_API.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string OrderCode { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public string PaymentStatus { get; set; } = "Unpaid";

        public string ShippingAddress { get; set; } = string.Empty;

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverPhone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = [];

        public ICollection<Payment> Payments { get; set; } = [];
    }
}
