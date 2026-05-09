using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO
{
    public class RevokeRoleDto
    {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role name is required!")]
        public string RoleName { get; set; }
    }
}
