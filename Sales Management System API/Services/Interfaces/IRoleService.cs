using Sales_Management_System_API.DTO;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<string> AssignRoleAsync(AssignRoleDto dto);
        Task<string> RevokeRoleAsync(RevokeRoleDto dto);
    }
}
