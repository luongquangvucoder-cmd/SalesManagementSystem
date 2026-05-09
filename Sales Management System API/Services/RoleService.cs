using Microsoft.AspNetCore.Identity;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(IRoleRepository roleRepository,
            UserManager<ApplicationUser> userManager)
        {
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            if (!roles.Any())
            {
                throw new NotFoundException("No roles found");
            }

            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name!
            });
        }

        public async Task<string> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new NotFoundException($"User '{dto.Email}' not found");
            }

            var roleExists = await _roleRepository.RoleExistsAsync(dto.RoleName);
            if (!roleExists)
            {
                throw new NotFoundException($"Role '{dto.RoleName}' not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Nếu đã có đúng role này rồi thì không cần làm gì
            if (currentRoles.Count == 1 && currentRoles.Contains(dto.RoleName))
            {
                throw new ConflictException($"User already has role '{dto.RoleName}'");
            }

            // Gỡ hết role cũ
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    throw new BadRequestException(string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                }
            }

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return $"Role '{dto.RoleName}' assigned to '{dto.Email}'";
        }

        public async Task<string> RevokeRoleAsync(RevokeRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new NotFoundException($"User '{dto.Email}' not found");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, dto.RoleName);
            if (!isInRole)
            {
                throw new BadRequestException($"User does not have role '{dto.RoleName}'");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return $"Role '{dto.RoleName}' revoked from '{dto.Email}'";
        }
    }
}
