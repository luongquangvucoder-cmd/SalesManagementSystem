using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.InventoryDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IInventoryService
    {
        // Transaction queries
        Task<PagedResult<InventoryTransactionDto>> GetAllTransactionsAsync(InventoryQueryDto query);
        Task<List<InventoryTransactionDto>> GetTransactionsByVariantAsync(int productVariantId);
        Task<InventoryTransactionDto> GetTransactionByIdAsync(int transactionId);

        // Transaction operations
        Task<InventoryTransactionDto> CreateTransactionAsync(CreateInventoryTransactionDto dto);
        Task<InventoryTransactionDto> ImportStockAsync(int productVariantId, int quantity, string? note = null);
        Task<InventoryTransactionDto> ExportStockAsync(int productVariantId, int quantity, string? note = null);
        Task<InventoryTransactionDto> AdjustStockAsync(int productVariantId, int quantity, string? note = null);
        Task<InventoryTransactionDto> ProcessReturnAsync(int productVariantId, int quantity, string? note = null);

        // Stock queries
        Task<int> GetCurrentStockAsync(int productVariantId);
        Task<List<StockLevelDto>> GetLowStockProductsAsync(int minimumThreshold = 10);
    }
}
