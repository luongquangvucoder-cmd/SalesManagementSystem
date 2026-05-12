using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.AddressDtos;
using Sales_Management_System_API.Services.Interfaces;
using System.Security.Claims;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = GetUserId();
            var result = await _addressService.GetAddressesByUserIdAsync(userId);
            return Ok(new ApiResponse<List<AddressDto>>
            {
                Success = true,
                Message = "Addresses retrieved successfully",
                Data = result
            });
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var userId = GetUserId();
            var result = await _addressService.GetDefaultAddressByUserIdAsync(userId);
            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Default address retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _addressService.GetByIdAsync(id);
            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Address retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _addressService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.AddressId },
                new ApiResponse<AddressDto>
                {
                    Success = true,
                    Message = "Address created successfully",
                    Data = result
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _addressService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Address updated successfully",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            await _addressService.DeleteAsync(id);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Address deleted successfully"
            });
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = GetUserId();
            await _addressService.SetDefaultAddressAsync(userId, id);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Address set as default successfully"
            });
        }
    }
}
