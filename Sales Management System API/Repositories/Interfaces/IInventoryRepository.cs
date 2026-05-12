using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.InventoryDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        // Transaction queries
        Task<PagedResult<InventoryTransactionDto>> GetAllTransactionsAsync(InventoryQueryDto query);
        Task<List<InventoryTransactionDto>> GetByProductVariantIdAsync(int productVariantId);
        Task<InventoryTransactionDto?> GetTransactionByIdAsync(int transactionId);

        // Transaction operations
        Task<InventoryTransactionDto> CreateTransactionAsync(InventoryTransaction transaction);

        // Stock operations
        Task UpdateStockAsync(int productVariantId, int quantityChange);
        Task<int> GetCurrentStockAsync(int productVariantId);
    }
}
