using Sales_Management_System_API.DTO.AddressDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        #region Address Queries

        public async Task<List<AddressDto>> GetAddressesByUserIdAsync(string userId)
        {
            return await _addressRepository.GetByUserIdAsync(userId);
        }

        public async Task<AddressDto> GetByIdAsync(int addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null)
                throw new NotFoundException($"Address with id {addressId} not found");

            return address;
        }

        public async Task<AddressDto> GetDefaultAddressByUserIdAsync(string userId)
        {
            var address = await _addressRepository.GetDefaultAddressByUserIdAsync(userId);
            if (address == null)
                throw new NotFoundException($"No default address found for user {userId}");

            return address;
        }

        #endregion

        #region Address Operations

        public async Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto)
        {
            // Validate format
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException("UserId is required");

            // If this is the first address or IsDefault is true, set it as default
            var userAddresses = await _addressRepository.GetByUserIdAsync(userId);
            bool shouldBeDefault = dto.IsDefault || !userAddresses.Any();

            var address = new Address
            {
                UserId = userId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,
                DetailAddress = dto.DetailAddress,
                IsDefault = shouldBeDefault
            };

            var createdAddress = await _addressRepository.CreateAsync(address);

            // If this is default, remove default from others
            if (shouldBeDefault)
            {
                var otherAddresses = userAddresses.Where(a => a.AddressId != createdAddress.AddressId).ToList();
                foreach (var otherAddr in otherAddresses)
                {
                    var otherEntity = await _addressRepository.GetAddressEntityByIdAsync(otherAddr.AddressId);
                    if (otherEntity != null)
                    {
                        otherEntity.IsDefault = false;
                        await _addressRepository.UpdateAsync(otherEntity);
                    }
                }
            }

            return createdAddress;
        }

        public async Task<AddressDto> UpdateAsync(int addressId, UpdateAddressDto dto)
        {
            var address = await _addressRepository.GetAddressEntityByIdAsync(addressId);
            if (address == null)
                throw new NotFoundException($"Address with id {addressId} not found");

            address.FullName = dto.FullName;
            address.PhoneNumber = dto.PhoneNumber;
            address.City = dto.City;
            address.District = dto.District;
            address.Ward = dto.Ward;
            address.DetailAddress = dto.DetailAddress;

            // If setting as default, remove default from others
            if (dto.IsDefault && !address.IsDefault)
            {
                await _addressRepository.SetDefaultAddressAsync(address.UserId, addressId);
                address.IsDefault = true;
            }
            else if (!dto.IsDefault && address.IsDefault)
            {
                // Prevent removing default if it's the only address
                var userAddresses = await _addressRepository.GetByUserIdAsync(address.UserId);
                if (userAddresses.Count == 1)
                    throw new BadRequestException("Cannot remove default status from the only address");

                address.IsDefault = false;
            }

            return await _addressRepository.UpdateAsync(address);
        }

        public async Task DeleteAsync(int addressId)
        {
            var address = await _addressRepository.GetAddressEntityByIdAsync(addressId);
            if (address == null)
                throw new NotFoundException($"Address with id {addressId} not found");

            // Check if this is the only address
            var userAddresses = await _addressRepository.GetByUserIdAsync(address.UserId);
            if (userAddresses.Count == 1)
                throw new BadRequestException("Cannot delete the only address");

            // If this is default, set another as default
            if (address.IsDefault)
            {
                var nextDefault = userAddresses.FirstOrDefault(a => a.AddressId != addressId);
                if (nextDefault != null)
                {
                    await _addressRepository.SetDefaultAddressAsync(address.UserId, nextDefault.AddressId);
                }
            }

            await _addressRepository.DeleteAsync(addressId);
        }

        public async Task SetDefaultAddressAsync(string userId, int addressId)
        {
            var address = await _addressRepository.GetAddressEntityByIdAsync(addressId);
            if (address == null)
                throw new NotFoundException($"Address with id {addressId} not found");

            if (address.UserId != userId)
                throw new ForbiddenException("You don't have permission to update this address");

            await _addressRepository.SetDefaultAddressAsync(userId, addressId);
        }

        #endregion
    }
}
