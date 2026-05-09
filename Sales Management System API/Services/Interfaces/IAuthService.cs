using Sales_Management_System_API.DTO;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);

        Task<string> ConfirmEmailAsync(string email, string token);

        Task<AuthResultDto> LoginAsync(LoginDto loginDto);

        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto request);

        Task<string> LogoutAsync(LogoutDto request);

        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
