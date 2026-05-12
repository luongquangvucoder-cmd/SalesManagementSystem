using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.ProductDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        // Product queries
        Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> IsProductNameUniqueAsync(string name, int? excludeId = null);
        Task<bool> HasActiveVariantsAsync(int productId);
        Task<List<string>> GetBrandsAsync();

        // Product operations
        Task<ProductDto> CreateAsync(Product product);
        Task<ProductDto> UpdateAsync(Product product);
        Task UpdateStatusAsync(int productId, bool status);
        Task DeleteAsync(int productId);

        // Variant queries
        Task<ProductVariantDto?> GetVariantByIdAsync(int variantId);
        Task<ProductVariantDto?> GetVariantBySkuAsync(string sku);
        Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);
        Task<ProductVariant?> GetVariantEntityByIdAsync(int variantId);

        // Variant operations
        Task<ProductVariantDto> CreateVariantAsync(ProductVariant variant);
        Task<ProductVariantDto> UpdateVariantAsync(ProductVariant variant);
        Task DeleteVariantAsync(int variantId);

        // Image queries
        Task<ProductImageDto?> GetImageByIdAsync(int imageId);
        Task<List<ProductImageDto>> GetImagesByProductIdAsync(int productId);

        // Image operations
        Task<ProductImageDto> CreateImageAsync(ProductImage image);
        Task UpdateImageAsync(ProductImage image);
        Task UpdateImagePrimaryStatusAsync(int imageId, bool isPrimary);
        Task DeleteImageAsync(int imageId);
    }
}
