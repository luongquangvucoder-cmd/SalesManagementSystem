using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);

        Task SaveChangesAsync();

        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
