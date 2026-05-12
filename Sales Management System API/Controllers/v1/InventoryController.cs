using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.InventoryDtos;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] InventoryQueryDto query)
        {
            var result = await _inventoryService.GetAllTransactionsAsync(query);
            return Ok(new ApiResponse<PagedResult<InventoryTransactionDto>>
            {
                Success = true,
                Message = "Inventory transactions retrieved successfully",
                Data = result
            });
        }

        [HttpGet("transactions/variant/{variantId}")]
        public async Task<IActionResult> GetTransactionsByVariant(int variantId)
        {
            var result = await _inventoryService.GetTransactionsByVariantAsync(variantId);
            return Ok(new ApiResponse<List<InventoryTransactionDto>>
            {
                Success = true,
                Message = "Inventory transactions retrieved successfully",
                Data = result
            });
        }

        [HttpGet("transactions/{transactionId}")]
        public async Task<IActionResult> GetTransactionById(int transactionId)
        {
            var result = await _inventoryService.GetTransactionByIdAsync(transactionId);
            return Ok(new ApiResponse<InventoryTransactionDto>
            {
                Success = true,
                Message = "Inventory transaction retrieved successfully",
                Data = result
            });
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateInventoryTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _inventoryService.CreateTransactionAsync(dto);
            return CreatedAtAction(nameof(GetTransactionById), new { transactionId = result.InventoryTransactionId },
                new ApiResponse<InventoryTransactionDto>
                {
                    Success = true,
                    Message = "Inventory transaction created successfully",
                    Data = result
                });
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStock([FromQuery] int variantId, [FromQuery] int quantity, [FromQuery] string? note = null)
        {
            var result = await _inventoryService.ImportStockAsync(variantId, quantity, note);
            return Ok(new ApiResponse<InventoryTransactionDto>
            {
                Success = true,
                Message = "Stock imported successfully",
                Data = result
            });
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportStock([FromQuery] int variantId, [FromQuery] int quantity, [FromQuery] string? note = null)
        {
            var result = await _inventoryService.ExportStockAsync(variantId, quantity, note);
            return Ok(new ApiResponse<InventoryTransactionDto>
            {
                Success = true,
                Message = "Stock exported successfully",
                Data = result
            });
        }

        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock([FromQuery] int variantId, [FromQuery] int quantity, [FromQuery] string? note = null)
        {
            var result = await _inventoryService.AdjustStockAsync(variantId, quantity, note);
            return Ok(new ApiResponse<InventoryTransactionDto>
            {
                Success = true,
                Message = "Stock adjusted successfully",
                Data = result
            });
        }

        [HttpPost("return")]
        public async Task<IActionResult> ProcessReturn([FromQuery] int variantId, [FromQuery] int quantity, [FromQuery] string? note = null)
        {
            var result = await _inventoryService.ProcessReturnAsync(variantId, quantity, note);
            return Ok(new ApiResponse<InventoryTransactionDto>
            {
                Success = true,
                Message = "Stock return processed successfully",
                Data = result
            });
        }

        [HttpGet("stock/{variantId}")]
        public async Task<IActionResult> GetCurrentStock(int variantId)
        {
            var stock = await _inventoryService.GetCurrentStockAsync(variantId);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Current stock retrieved successfully",
                Data = stock
            });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var result = await _inventoryService.GetLowStockProductsAsync(threshold);
            return Ok(new ApiResponse<List<StockLevelDto>>
            {
                Success = true,
                Message = "Low stock products retrieved successfully",
                Data = result
            });
        }
    }
}
