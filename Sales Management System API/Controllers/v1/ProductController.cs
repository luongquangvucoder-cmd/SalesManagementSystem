using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.ProductDtos;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        {
            var result = await _productService.GetAllAsync(query);
            return Ok(new ApiResponse<PagedResult<ProductDto>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result
            });
        }

        [HttpGet("brands")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _productService.GetBrandsAsync();
            return Ok(new ApiResponse<List<string>>
            {
                Success = true,
                Message = "Brands retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId },
                new ApiResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = result
                });
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }

        #region Variants

        [HttpPost("{productId}/variants")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVariant(int productId, [FromBody] CreateProductVariantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateVariantAsync(productId, dto);
            return CreatedAtAction(nameof(GetVariant), new { productId, variantId = result.ProductVariantId },
                new ApiResponse<ProductVariantDto>
                {
                    Success = true,
                    Message = "Product variant created successfully",
                    Data = result
                });
        }

        [HttpGet("{productId}/variants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVariants(int productId)
        {
            var result = await _productService.GetVariantsByProductIdAsync(productId);
            return Ok(new ApiResponse<List<ProductVariantDto>>
            {
                Success = true,
                Message = "Product variants retrieved successfully",
                Data = result
            });
        }

        [HttpPut("variants/{variantId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateVariantAsync(variantId, dto);
            return Ok(new ApiResponse<ProductVariantDto>
            {
                Success = true,
                Message = "Product variant updated successfully",
                Data = result
            });
        }

        [HttpDelete("variants/{variantId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVariant(int variantId)
        {
            await _productService.DeleteVariantAsync(variantId);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Product variant deleted successfully"
            });
        }

        [HttpGet("{productId}/variants/{variantId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVariant(int productId, int variantId)
        {
            var variants = await _productService.GetVariantsByProductIdAsync(productId);
            var variant = variants.FirstOrDefault(v => v.ProductVariantId == variantId);

            if (variant == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Variant not found"
                });

            return Ok(new ApiResponse<ProductVariantDto>
            {
                Success = true,
                Message = "Product variant retrieved successfully",
                Data = variant
            });
        }

        #endregion

        #region Images

        [HttpPost("{productId}/images")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddImage(int productId, [FromBody] CreateProductImageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.AddImageAsync(productId, dto);
            return CreatedAtAction(nameof(GetImage), new { productId, imageId = result.ProductImageId },
                new ApiResponse<ProductImageDto>
                {
                    Success = true,
                    Message = "Product image added successfully",
                    Data = result
                });
        }

        [HttpGet("{productId}/images")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages(int productId)
        {
            var result = await _productService.GetImagesByProductIdAsync(productId);
            return Ok(new ApiResponse<List<ProductImageDto>>
            {
                Success = true,
                Message = "Product images retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{productId}/images/{imageId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int productId, int imageId)
        {
            var images = await _productService.GetImagesByProductIdAsync(productId);
            var image = images.FirstOrDefault(i => i.ProductImageId == imageId);

            if (image == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Image not found"
                });

            return Ok(new ApiResponse<ProductImageDto>
            {
                Success = true,
                Message = "Product image retrieved successfully",
                Data = image
            });
        }

        [HttpPut("{productId}/images/{imageId}/set-primary")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPrimaryImage(int productId, int imageId)
        {
            await _productService.SetPrimaryImageAsync(imageId);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Image set as primary successfully"
            });
        }

        [HttpDelete("{productId}/images/{imageId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            await _productService.DeleteImageAsync(imageId);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Product image deleted successfully"
            });
        }

        #endregion
    }
}
