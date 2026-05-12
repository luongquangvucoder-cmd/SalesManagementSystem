using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.PaymentDtos
{
    // Tr? v? client
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // T?o thanh toán
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "OrderId is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "PaymentMethod must be between 1 and 100 characters")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionCode { get; set; }
    }

    // C?p nh?t tr?ng thái
    public class UpdatePaymentStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string Status { get; set; } = string.Empty;
    }

    // Payment Method enum
    public class PaymentMethodConstants
    {
        public const string COD = "COD"; // Cash on Delivery
        public const string BankTransfer = "BankTransfer";
        public const string CreditCard = "CreditCard";
        public const string MoMo = "MoMo";
        public const string VNPay = "VNPay";
        public const string Stripe = "Stripe";

        public static List<string> GetAllMethods() => new()
        {
            COD,
            BankTransfer,
            CreditCard,
            MoMo,
            VNPay,
            Stripe
        };
    }

    // Payment Status enum
    public class PaymentStatusConstants
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";

        public static List<string> GetAllStatuses() => new()
        {
            Pending,
            Processing,
            Completed,
            Failed,
            Cancelled,
            Refunded
        };
    }
}
