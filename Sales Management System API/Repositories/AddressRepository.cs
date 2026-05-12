using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO.AddressDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Address Queries

        public async Task<List<AddressDto>> GetByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .AsNoTracking()
                .Select(a => MapToAddressDto(a))
                .ToListAsync();
        }

        public async Task<AddressDto?> GetByIdAsync(int addressId)
        {
            var address = await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AddressId == addressId);

            return address == null ? null : MapToAddressDto(address);
        }

        public async Task<Address?> GetAddressEntityByIdAsync(int addressId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId);
        }

        public async Task<AddressDto?> GetDefaultAddressByUserIdAsync(string userId)
        {
            var address = await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

            return address == null ? null : MapToAddressDto(address);
        }

        #endregion

        #region Address Operations

        public async Task<AddressDto> CreateAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            var createdAddress = await GetByIdAsync(address.AddressId);
            return createdAddress!;
        }

        public async Task<AddressDto> UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            var updatedAddress = await GetByIdAsync(address.AddressId);
            return updatedAddress!;
        }

        public async Task DeleteAsync(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetDefaultAddressAsync(string userId, int addressId)
        {
            // Remove default from all other addresses
            var currentDefault = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

            if (currentDefault != null)
            {
                currentDefault.IsDefault = false;
            }

            // Set new default
            var newDefault = await _context.Addresses.FindAsync(addressId);
            if (newDefault != null && newDefault.UserId == userId)
            {
                newDefault.IsDefault = true;
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Helper Methods

        private static AddressDto MapToAddressDto(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                City = address.City,
                District = address.District,
                Ward = address.Ward,
                DetailAddress = address.DetailAddress,
                IsDefault = address.IsDefault
            };
        }

        #endregion
    }
}
