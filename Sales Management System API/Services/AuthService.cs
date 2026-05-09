using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Sales_Management_System_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailService _emailService;
        private readonly IRoleRepository _roleRepository;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IEmailService emailService,
            IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
            _roleRepository = roleRepository;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var roleExists = await _roleRepository.RoleExistsAsync(registerDto.RoleName);
            if (!roleExists)
            {
                throw new NotFoundException($"Role '{registerDto.RoleName}' not found");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, registerDto.RoleName);
            if (!roleResult.Succeeded)
            {
                // Rollback user nếu gán role thất bại
                await _userManager.DeleteAsync(user);
                throw new BadRequestException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var confirmationLink = $"https://localhost:7135/verify-email?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Verify your email",
                $@"
                    <h2>Email Verification</h2>
                    <p>Please click the link below to verify your account:</p>
                    <a href='{confirmationLink}'>Verify Email</a>
                "
            );

            return $"Verification email sent to {registerDto.Email}";
        }

        public async Task<string> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new NotFoundException("Invalid email");
            }

            var decodedToken = HttpUtility.UrlDecode(token);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                throw new UnprocessableEntityException("Invalid or expired confirmation token");
            }

            return "Email confirmed successfully";
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            ApplicationUser? user;

            if (loginDto.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail);
            }

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username/email or password");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Incorrect password");
            }

            if (!user.EmailConfirmed)
            {
                throw new ForbiddenException("Please verify your email");
            }

            return await GenerateJwtToken(user);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto request)
        {
            var principal = GetPrincipalFromToken(request.AccessToken);

            var jwtId = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jwtId == null)
            {
                throw new UnauthorizedException("Invalid token");
            }

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null)
            {
                throw new UnauthorizedException("Refresh token not found");
            }

            if (storedToken.IsUsed)
            {
                throw new UnauthorizedException("Refresh token already used");
            }

            if (storedToken.IsRevoked)
            {
                throw new UnauthorizedException("Refresh token revoked");
            }

            if (storedToken.DateExpire < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token expired");
            }

            if (storedToken.JwtId != jwtId)
            {
                throw new UnauthorizedException("Token does not match");
            }

            storedToken.IsUsed = true;

            await _refreshTokenRepository.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return await GenerateJwtToken(user);
        }

        public async Task<string> LogoutAsync(LogoutDto request)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            if (storedToken.IsRevoked)
            {
                throw new UnauthorizedException("Token already revoked");
            }

            storedToken.IsRevoked = true;

            await _refreshTokenRepository.SaveChangesAsync();

            return "Logout successful";
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                throw new NotFoundException("If the email exists, a reset link has been sent");
            }

            if (!user.EmailConfirmed)
            {
                throw new ForbiddenException("Please verify your email first");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = HttpUtility.UrlEncode(token);

            var resetLink = $"https://localhost:7135/reset-password?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                 user.Email!,
                 "Reset Password",
                 $@"
                    <h2>Reset Password</h2>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                "
             );

            return "Password reset link sent to email";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
            {
                throw new NotFoundException("Invalid request");
            }

            var decodedToken = HttpUtility.UrlDecode(resetPasswordDto.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                throw new BadRequestException(string.Join(", ", errors));
            }

            return "Password reset successful";
        }

        private async Task<AuthResultDto> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                IsUsed = false,
                IsRevoked = false,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResultDto()
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,

                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedException("Invalid token");
            }

            return principal;
        }
    }
}
