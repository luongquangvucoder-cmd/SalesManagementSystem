using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.CategoryDtos;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] CategoryQueryDto query)
        {
            var result = await _categoryService.GetAllAsync(query);
            return Ok(new ApiResponse<PagedResult<CategoryDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = result
            });
        }

        [HttpGet("tree")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTree()
        {
            var result = await _categoryService.GetTreeAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryTreeDto>>
            {
                Success = true,
                Message = "Category tree retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CategoryId },
                new ApiResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Category created successfully",
                    Data = result
                });
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Category deactivated successfully"
            });
        }
    }
}
