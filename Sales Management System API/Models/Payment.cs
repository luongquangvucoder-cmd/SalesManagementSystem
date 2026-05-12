namespace Sales_Management_System_API.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? TransactionCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
