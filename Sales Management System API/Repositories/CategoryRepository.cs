using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CategoryDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Category>> GetAllAsync(CategoryQueryDto query)
        {
            var q = _context.Categories
                .Include(c => c.Parent)
                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(c => c.Name.Contains(query.Search));

            // Lọc theo cha
            if (query.ParentId.HasValue)
                q = q.Where(c => c.ParentId == query.ParentId);

            // Lọc theo trạng thái
            if (query.IsActive.HasValue)
                q = q.Where(c => c.IsActive == query.IsActive);

            var totalCount = await q.CountAsync();

            var items = await q
                .OrderBy(c => c.ParentId)
                .ThenBy(c => c.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        // Lấy toàn bộ cây — không phân trang
        public async Task<IEnumerable<Category>> GetTreeAsync()
        {
            return await _context.Categories
                .Include(c => c.Children)
                .Where(c => c.ParentId == null && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == id);
        }

        // Kiểm tra tên trùng — bỏ qua chính nó khi update
        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Categories.AnyAsync(c =>
                c.Name == name &&
                (!excludeId.HasValue || c.CategoryId != excludeId));
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
