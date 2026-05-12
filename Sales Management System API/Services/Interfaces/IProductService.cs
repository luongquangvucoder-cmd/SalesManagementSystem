using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.ProductDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IProductService
    {
        // Product queries
        Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query);
        Task<ProductDto> GetByIdAsync(int id);
        Task<List<string>> GetBrandsAsync();

        // Product operations
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);

        // Variant operations
        Task<ProductVariantDto> CreateVariantAsync(int productId, CreateProductVariantDto dto);
        Task<ProductVariantDto> UpdateVariantAsync(int variantId, UpdateProductVariantDto dto);
        Task DeleteVariantAsync(int variantId);
        Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);

        // Image operations
        Task<ProductImageDto> AddImageAsync(int productId, CreateProductImageDto dto);
        Task DeleteImageAsync(int imageId);
        Task<List<ProductImageDto>> GetImagesByProductIdAsync(int productId);
        Task SetPrimaryImageAsync(int imageId);
    }
}
