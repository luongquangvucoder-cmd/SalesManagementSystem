using Sales_Management_System_API.DTO;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);

        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
}
