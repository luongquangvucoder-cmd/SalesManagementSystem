namespace Sales_Management_System_API.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public string DetailAddress { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
