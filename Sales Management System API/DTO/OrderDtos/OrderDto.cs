using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.OrderDtos
{
    // Tr? v? client
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = [];
    }

    // Order Item
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    // T?o m?i order t? cart
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "ShippingAddress is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "ShippingAddress must be between 10 and 500 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "ReceiverName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ReceiverName must be between 2 and 100 characters")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ReceiverPhone is required")]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "ReceiverPhone must be between 10 and 20 digits")]
        public string ReceiverPhone { get; set; } = string.Empty;
    }

    // C?p nh?t status
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "OrderStatus is required")]
        [StringLength(50, ErrorMessage = "OrderStatus must not exceed 50 characters")]
        public string OrderStatus { get; set; } = string.Empty;
    }

    // C?p nh?t payment status
    public class UpdatePaymentStatusDto
    {
        [Required(ErrorMessage = "PaymentStatus is required")]
        [StringLength(50, ErrorMessage = "PaymentStatus must not exceed 50 characters")]
        public string PaymentStatus { get; set; } = string.Empty;
    }

    // Phân trang + tìm ki?m
    public class OrderQueryDto
    {
        public string? OrderCode { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }

    // Order Status enum
    public class OrderStatusConstants
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Shipping = "Shipping";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static List<string> GetAllStatuses() => new()
        {
            Pending,
            Confirmed,
            Shipping,
            Completed,
            Cancelled
        };
    }

    // Payment Status enum
    public class PaymentStatusConstants
    {
        public const string Unpaid = "Unpaid";
        public const string Partial = "Partial";
        public const string Paid = "Paid";
        public const string Refunded = "Refunded";
        public const string Failed = "Failed";

        public static List<string> GetAllStatuses() => new()
        {
            Unpaid,
            Partial,
            Paid,
            Refunded,
            Failed
        };
    }
}
