namespace Sales_Management_System_API.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Product Product { get; set; } = null!;
        //public ICollection<VariantAttribute> VariantAttributes { get; set; } = [];
    }
}
