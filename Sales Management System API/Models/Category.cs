namespace Sales_Management_System_API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public bool IsActive { get; set; } = true;  
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; } = [];

        public ICollection<Product> Products { get; set; } = [];
    }
}
