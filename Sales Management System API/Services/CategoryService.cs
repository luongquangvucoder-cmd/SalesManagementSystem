using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CategoryDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResult<CategoryDto>> GetAllAsync(CategoryQueryDto query)
        {
            var paged = await _categoryRepository.GetAllAsync(query);

            return new PagedResult<CategoryDto>
            {
                Items = (List<CategoryDto>)paged.Items.Select(ToDto),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync()
        {
            var roots = await _categoryRepository.GetTreeAsync();
            return roots.Select(ToTreeDto);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Category with id '{id}' not found");

            return ToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            // Kiểm tra tên trùng
            var nameExists = await _categoryRepository.NameExistsAsync(dto.Name);
            if (nameExists)
                throw new ConflictException($"Category '{dto.Name}' already exists");

            // Kiểm tra ParentId có tồn tại không
            if (dto.ParentId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsAsync(dto.ParentId.Value);
                if (!parentExists)
                    throw new NotFoundException($"Parent category with id '{dto.ParentId}' not found");
            }

            var category = new Category
            {
                Name = dto.Name,
                ParentId = dto.ParentId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return await GetByIdAsync(category.CategoryId);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Category with id '{id}' not found");

            // Kiểm tra tên trùng — bỏ qua chính nó
            var nameExists = await _categoryRepository.NameExistsAsync(dto.Name, excludeId: id);
            if (nameExists)
                throw new ConflictException($"Category '{dto.Name}' already exists");

            // Kiểm tra ParentId có tồn tại không
            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId == id)
                    throw new BadRequestException("Category cannot be its own parent");

                var parentExists = await _categoryRepository.ExistsAsync(dto.ParentId.Value);
                if (!parentExists)
                    throw new NotFoundException($"Parent category with id '{dto.ParentId}' not found");
            }

            category.Name = dto.Name;
            category.ParentId = dto.ParentId;

            await _categoryRepository.SaveChangesAsync();

            return await GetByIdAsync(category.CategoryId);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Category with id '{id}' not found");

            if (!category.IsActive)
                throw new BadRequestException("Category is already inactive");

            // Kiểm tra có category con đang active không
            var hasActiveChildren = category.Children.Any(c => c.IsActive);
            if (hasActiveChildren)
                throw new BadRequestException("Cannot deactivate category that has active subcategories");

            category.IsActive = false;
            await _categoryRepository.SaveChangesAsync();
        }

        // Mapping
        private static CategoryDto ToDto(Category c) => new()
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            ParentId = c.ParentId,
            ParentName = c.Parent?.Name,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        private static CategoryTreeDto ToTreeDto(Category c) => new()
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            IsActive = c.IsActive,
            Children = c.Children
                .Where(child => child.IsActive)
                .OrderBy(child => child.Name)
                .Select(ToTreeDto)
                .ToList()
        };
    }
}
