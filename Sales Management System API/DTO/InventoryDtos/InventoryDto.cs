using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.InventoryDtos
{
    public class InventoryTransactionDto
    {
        public int InventoryTransactionId { get; set; }
        public int ProductVariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int QuantityChanged { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateInventoryTransactionDto
    {
        [Required(ErrorMessage = "ProductVariantId is required")]
        public int ProductVariantId { get; set; }

        [Required(ErrorMessage = "QuantityChanged is required")]
        public int QuantityChanged { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, MinimumLength = 1)]
        public string Type { get; set; } = string.Empty; // "Import", "Export", "Adjustment", "Return"

        [StringLength(500)]
        public string? Note { get; set; }
    }

    public class InventoryQueryDto
    {
        public int? ProductVariantId { get; set; }
        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class InventoryTypeConstants
    {
        public const string Import = "Import";
        public const string Export = "Export";
        public const string Adjustment = "Adjustment";
        public const string Return = "Return";

        public static List<string> GetAllTypes() => new()
        {
            Import,
            Export,
            Adjustment,
            Return
        };
    }

    public class StockLevelDto
    {
        public int ProductVariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public bool IsLowStock => CurrentStock <= MinimumStock;
    }
}
