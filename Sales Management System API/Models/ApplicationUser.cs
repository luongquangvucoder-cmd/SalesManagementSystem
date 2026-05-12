using Microsoft.AspNetCore.Identity;

namespace Sales_Management_System_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
