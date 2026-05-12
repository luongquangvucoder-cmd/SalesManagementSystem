using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CategoryDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<PagedResult<Category>> GetAllAsync(CategoryQueryDto query);
        Task<IEnumerable<Category>> GetTreeAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task AddAsync(Category category);
        Task SaveChangesAsync();
    }
}
