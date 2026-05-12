namespace Sales_Management_System_API.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Brand { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category Category { get; set; } = null!;

        public ICollection<ProductVariant> Variants { get; set; } = [];

        public ICollection<ProductImage> Images { get; set; } = [];
    }
}
