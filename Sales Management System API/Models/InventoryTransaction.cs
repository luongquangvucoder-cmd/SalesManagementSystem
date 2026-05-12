namespace Sales_Management_System_API.Models
{
    public class InventoryTransaction
    {
        public int InventoryTransactionId { get; set; }

        public int ProductVariantId { get; set; }

        public int QuantityChanged { get; set; }

        public int StockBefore { get; set; }

        public int StockAfter { get; set; }

        public string Type { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
