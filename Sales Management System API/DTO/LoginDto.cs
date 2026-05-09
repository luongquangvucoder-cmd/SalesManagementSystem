using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "UserName or Email  is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
