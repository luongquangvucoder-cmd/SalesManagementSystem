using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<RoleDto>>
            {
                Success = true,
                Message = "Roles retrieved successfully",
                Data = roles
            });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.AssignRoleAsync(dto);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = result
            });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.RevokeRoleAsync(dto);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = result
            });
        }
    }
}
