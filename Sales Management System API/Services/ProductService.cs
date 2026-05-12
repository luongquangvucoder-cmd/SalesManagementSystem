using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.ProductDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        #region Product Queries

        public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query)
        {
            if (query.Page < 1)
                throw new BadRequestException("Page must be greater than 0");

            if (query.PageSize < 1 || query.PageSize > 100)
                throw new BadRequestException("PageSize must be between 1 and 100");

            return await _productRepository.GetAllAsync(query);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found");

            return product;
        }

        public async Task<List<string>> GetBrandsAsync()
        {
            var brands = await _productRepository.GetBrandsAsync();
            return brands;
        }

        #endregion

        #region Product Operations

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                throw new BadRequestException($"Category with id {dto.CategoryId} not found");

            // Validate category is active
            if (!category.IsActive)
                throw new BadRequestException("Category is inactive");

            // Check if product name is unique
            var isUnique = await _productRepository.IsProductNameUniqueAsync(dto.Name);
            if (!isUnique)
                throw new ConflictException("Product name already exists");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Brand = dto.Brand,
                CategoryId = dto.CategoryId,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _productRepository.CreateAsync(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            // Get existing product
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found");

            // Validate category exists and is active
            if (product.CategoryId != dto.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
                if (category == null)
                    throw new BadRequestException($"Category with id {dto.CategoryId} not found");

                if (!category.IsActive)
                    throw new BadRequestException("Category is inactive");
            }

            // Check if new product name is unique
            if (product.Name != dto.Name)
            {
                var isUnique = await _productRepository.IsProductNameUniqueAsync(dto.Name, id);
                if (!isUnique)
                    throw new ConflictException("Product name already exists");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Brand = dto.Brand;
            product.CategoryId = dto.CategoryId;
            product.Status = dto.Status;

            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found");

            // Delete product variants first (cascade will handle)
            await _productRepository.DeleteAsync(id);
        }

        #endregion

        #region Variant Operations

        public async Task<ProductVariantDto> CreateVariantAsync(int productId, CreateProductVariantDto dto)
        {
            // Validate product exists
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product with id {productId} not found");

            // Check if SKU is unique
            var existingVariant = await _productRepository.GetVariantBySkuAsync(dto.SKU);
            if (existingVariant != null)
                throw new ConflictException($"SKU '{dto.SKU}' already exists");

            var variant = new ProductVariant
            {
                ProductId = productId,
                SKU = dto.SKU,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CreatedAt = DateTime.UtcNow
            };

            return await _productRepository.CreateVariantAsync(variant);
        }

        public async Task<ProductVariantDto> UpdateVariantAsync(int variantId, UpdateProductVariantDto dto)
        {
            var variant = await _productRepository.GetVariantByIdAsync(variantId);
            if (variant == null)
                throw new NotFoundException($"Variant with id {variantId} not found");

            var variantEntity = await _productRepository.GetVariantEntityByIdAsync(variantId);
            if (variantEntity == null)
                throw new NotFoundException($"Variant with id {variantId} not found");

            variantEntity.Price = dto.Price;
            variantEntity.StockQuantity = dto.StockQuantity;

            return await _productRepository.UpdateVariantAsync(variantEntity);
        }

        public async Task DeleteVariantAsync(int variantId)
        {
            var variant = await _productRepository.GetVariantByIdAsync(variantId);
            if (variant == null)
                throw new NotFoundException($"Variant with id {variantId} not found");

            await _productRepository.DeleteVariantAsync(variantId);
        }

        public async Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product with id {productId} not found");

            return await _productRepository.GetVariantsByProductIdAsync(productId);
        }

        #endregion

        #region Image Operations

        public async Task<ProductImageDto> AddImageAsync(int productId, CreateProductImageDto dto)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product with id {productId} not found");

            // If this is primary, set others to non-primary
            if (dto.IsPrimary)
            {
                var existingImages = await _productRepository.GetImagesByProductIdAsync(productId);
                var primaryImage = existingImages.FirstOrDefault(i => i.IsPrimary);
                if (primaryImage != null)
                {
                    await _productRepository.UpdateImagePrimaryStatusAsync(primaryImage.ProductImageId, false);
                }
            }

            var image = new ProductImage
            {
                ProductId = productId,
                ImageUrl = dto.ImageUrl,
                IsPrimary = dto.IsPrimary
            };

            return await _productRepository.CreateImageAsync(image);
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _productRepository.GetImageByIdAsync(imageId);
            if (image == null)
                throw new NotFoundException($"Image with id {imageId} not found");

            await _productRepository.DeleteImageAsync(imageId);
        }

        public async Task<List<ProductImageDto>> GetImagesByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product with id {productId} not found");

            return await _productRepository.GetImagesByProductIdAsync(productId);
        }

        public async Task SetPrimaryImageAsync(int imageId)
        {
            var image = await _productRepository.GetImageByIdAsync(imageId);
            if (image == null)
                throw new NotFoundException($"Image with id {imageId} not found");

            // Set all images of this product to non-primary
            var allImages = await _productRepository.GetImagesByProductIdAsync(image.ProductId);
            foreach (var img in allImages)
            {
                if (img.ProductImageId != imageId)
                {
                    await _productRepository.UpdateImagePrimaryStatusAsync(img.ProductImageId, false);
                }
            }

            // Set this image as primary
            await _productRepository.UpdateImagePrimaryStatusAsync(imageId, true);
        }

        #endregion
    }
}
