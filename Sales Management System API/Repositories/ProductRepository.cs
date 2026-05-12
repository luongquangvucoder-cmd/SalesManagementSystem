using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.ProductDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Product Queries

        public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query)
        {
            var queryable = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .AsNoTracking();

            // Filter by search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchLower = query.Search.ToLower();
                queryable = queryable.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description!.ToLower().Contains(searchLower) ||
                    p.Brand!.ToLower().Contains(searchLower));
            }

            // Filter by category
            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            // Filter by brand
            if (!string.IsNullOrWhiteSpace(query.Brand))
            {
                queryable = queryable.Where(p => p.Brand == query.Brand);
            }

            // Filter by price range
            if (query.MinPrice.HasValue)
            {
                queryable = queryable.Where(p =>
                    p.Variants.Any(v => v.Price >= query.MinPrice.Value));
            }

            if (query.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p =>
                    p.Variants.Any(v => v.Price <= query.MaxPrice.Value));
            }

            // Filter by status
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(p => p.Status == query.Status.Value);
            }

            // Count total before pagination
            var totalCount = await queryable.CountAsync();

            // Sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending
                    ? queryable.OrderByDescending(p => p.Name)
                    : queryable.OrderBy(p => p.Name),
                "brand" => query.IsDescending
                    ? queryable.OrderByDescending(p => p.Brand)
                    : queryable.OrderBy(p => p.Brand),
                "price" => query.IsDescending
                    ? queryable.OrderByDescending(p => p.Variants.Min(v => v.Price))
                    : queryable.OrderBy(p => p.Variants.Min(v => v.Price)),
                _ => query.IsDescending
                    ? queryable.OrderByDescending(p => p.CreatedAt)
                    : queryable.OrderBy(p => p.CreatedAt)
            };

            // Pagination
            var skip = (query.Page - 1) * query.PageSize;
            var products = await queryable
                .Skip(skip)
                .Take(query.PageSize)
                .Select(p => MapToProductDto(p))
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return product == null ? null : MapToProductDto(product);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<bool> IsProductNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.ProductId != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> HasActiveVariantsAsync(int productId)
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId && v.StockQuantity > 0)
                .AnyAsync();
        }

        #endregion

        #region Product Operations

        public async Task<ProductDto> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdProduct = await GetByIdAsync(product.ProductId);
            return createdProduct!;
        }

        public async Task<ProductDto> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            var updatedProduct = await GetByIdAsync(product.ProductId);
            return updatedProduct!;
        }

        public async Task UpdateStatusAsync(int productId, bool status)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Variant Queries

        public async Task<ProductVariantDto?> GetVariantByIdAsync(int variantId)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ProductVariantId == variantId);

            return variant == null ? null : MapToVariantDto(variant);
        }

        public async Task<ProductVariantDto?> GetVariantBySkuAsync(string sku)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.SKU == sku);

            return variant == null ? null : MapToVariantDto(variant);
        }

        public async Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .AsNoTracking()
                .Select(v => MapToVariantDto(v))
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetVariantEntityByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductVariantId == variantId);
        }

        public async Task<List<string>> GetBrandsAsync()
        {
            return await _context.Products
                .Where(p => !string.IsNullOrWhiteSpace(p.Brand))
                .Select(p => p.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion

        #region Variant Operations

        public async Task<ProductVariantDto> CreateVariantAsync(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var createdVariant = await GetVariantByIdAsync(variant.ProductVariantId);
            return createdVariant!;
        }

        public async Task<ProductVariantDto> UpdateVariantAsync(ProductVariant variant)
        {
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();

            var updatedVariant = await GetVariantByIdAsync(variant.ProductVariantId);
            return updatedVariant!;
        }

        public async Task DeleteVariantAsync(int variantId)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Image Queries

        public async Task<ProductImageDto?> GetImageByIdAsync(int imageId)
        {
            var image = await _context.ProductImages
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ProductImageId == imageId);

            return image == null ? null : MapToImageDto(image);
        }

        public async Task<List<ProductImageDto>> GetImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .AsNoTracking()
                .Select(i => MapToImageDto(i))
                .ToListAsync();
        }

        #endregion

        #region Image Operations

        public async Task<ProductImageDto> CreateImageAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            var createdImage = await GetImageByIdAsync(image.ProductImageId);
            return createdImage!;
        }

        public async Task UpdateImageAsync(ProductImage image)
        {
            _context.ProductImages.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImagePrimaryStatusAsync(int imageId, bool isPrimary)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                image.IsPrimary = isPrimary;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Helper Methods

        private static ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                Variants = product.Variants.Select(MapToVariantDto).ToList(),
                Images = product.Images.Select(MapToImageDto).ToList()
            };
        }

        private static ProductVariantDto MapToVariantDto(ProductVariant variant)
        {
            return new ProductVariantDto
            {
                ProductVariantId = variant.ProductVariantId,
                ProductId = variant.ProductId,
                SKU = variant.SKU,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                CreatedAt = variant.CreatedAt
            };
        }

        private static ProductImageDto MapToImageDto(ProductImage image)
        {
            return new ProductImageDto
            {
                ProductImageId = image.ProductImageId,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl,
                IsPrimary = image.IsPrimary
            };
        }

        #endregion
    }
}
