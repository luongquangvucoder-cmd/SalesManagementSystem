using Sales_Management_System_API.DTO.AddressDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IAddressService
    {
        // Address queries
        Task<List<AddressDto>> GetAddressesByUserIdAsync(string userId);
        Task<AddressDto> GetByIdAsync(int addressId);
        Task<AddressDto> GetDefaultAddressByUserIdAsync(string userId);

        // Address operations
        Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto);
        Task<AddressDto> UpdateAsync(int addressId, UpdateAddressDto dto);
        Task DeleteAsync(int addressId);
        Task SetDefaultAddressAsync(string userId, int addressId);
    }
}
