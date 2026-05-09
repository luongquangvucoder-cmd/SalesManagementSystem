using Microsoft.AspNetCore.Identity;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<IdentityRole>> GetAllAsync();
        Task<bool> RoleExistsAsync(string roleName);
    }
}
