using Sales_Management_System_API.DTO.AddressDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        // Address queries
        Task<List<AddressDto>> GetByUserIdAsync(string userId);
        Task<AddressDto?> GetByIdAsync(int addressId);
        Task<Address?> GetAddressEntityByIdAsync(int addressId);
        Task<AddressDto?> GetDefaultAddressByUserIdAsync(string userId);

        // Address operations
        Task<AddressDto> CreateAsync(Address address);
        Task<AddressDto> UpdateAsync(Address address);
        Task DeleteAsync(int addressId);
        Task SetDefaultAddressAsync(string userId, int addressId);
    }
}
