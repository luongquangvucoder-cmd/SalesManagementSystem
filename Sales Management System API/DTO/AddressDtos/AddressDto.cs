using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.AddressDtos
{
    // Tr? v? client
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string DetailAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    // T?o m?i
    public class CreateAddressDto
    {
        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "PhoneNumber must be between 10 and 20 digits")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "District is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "District must be between 2 and 100 characters")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ward is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ward must be between 2 and 100 characters")]
        public string Ward { get; set; } = string.Empty;

        [Required(ErrorMessage = "DetailAddress is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "DetailAddress must be between 5 and 500 characters")]
        public string DetailAddress { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }

    // C?p nh?t
    public class UpdateAddressDto
    {
        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "PhoneNumber must be between 10 and 20 digits")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "District is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "District must be between 2 and 100 characters")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ward is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ward must be between 2 and 100 characters")]
        public string Ward { get; set; } = string.Empty;

        [Required(ErrorMessage = "DetailAddress is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "DetailAddress must be between 5 and 500 characters")]
        public string DetailAddress { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }
}
