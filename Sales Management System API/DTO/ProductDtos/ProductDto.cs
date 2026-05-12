using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.ProductDtos
{
    // Tr? v? client
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductVariantDto> Variants { get; set; } = [];
        public List<ProductImageDto> Images { get; set; } = [];
    }

    // Product Variant
    public class ProductVariantDto
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Product Image
    public class ProductImageDto
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    // T?o m?i
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "Description must not exceed 4000 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Brand must not exceed 100 characters")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }
    }

    // C?p nh?t
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "Description must not exceed 4000 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Brand must not exceed 100 characters")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        public bool Status { get; set; } = true;
    }

    // Phân trang + tìm ki?m
    public class ProductQueryDto
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }

    // Variant t?o/c?p nh?t
    public class CreateProductVariantDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 100 characters")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "StockQuantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }
    }

    public class UpdateProductVariantDto
    {
        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "StockQuantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }
    }

    // Image t?o/c?p nh?t
    public class CreateProductImageDto
    {
        [Required(ErrorMessage = "ImageUrl is required")]
        [StringLength(1000, ErrorMessage = "ImageUrl must not exceed 1000 characters")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;
    }
}
