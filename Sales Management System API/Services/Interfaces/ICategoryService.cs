using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CategoryDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(CategoryQueryDto query);
        Task<IEnumerable<CategoryTreeDto>> GetTreeAsync();
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
        Task DeleteAsync(int id);  // soft delete
    }
}
