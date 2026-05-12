using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.InventoryDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Transaction Queries

        public async Task<PagedResult<InventoryTransactionDto>> GetAllTransactionsAsync(InventoryQueryDto query)
        {
            var queryable = _context.InventoryTransactions
                .Include(it => it.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsNoTracking();

            // Filter by product variant
            if (query.ProductVariantId.HasValue)
            {
                queryable = queryable.Where(it => it.ProductVariantId == query.ProductVariantId.Value);
            }

            // Filter by type
            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                queryable = queryable.Where(it => it.Type == query.Type);
            }

            // Filter by date range
            if (query.FromDate.HasValue)
            {
                queryable = queryable.Where(it => it.CreatedAt >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                queryable = queryable.Where(it => it.CreatedAt <= query.ToDate.Value.AddDays(1));
            }

            var totalCount = await queryable.CountAsync();

            var transactions = await queryable
                .OrderByDescending(it => it.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(it => MapToDto(it))
                .ToListAsync();

            return new PagedResult<InventoryTransactionDto>
            {
                Items = transactions,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<List<InventoryTransactionDto>> GetByProductVariantIdAsync(int productVariantId)
        {
            return await _context.InventoryTransactions
                .Where(it => it.ProductVariantId == productVariantId)
                .Include(it => it.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsNoTracking()
                .OrderByDescending(it => it.CreatedAt)
                .Select(it => MapToDto(it))
                .ToListAsync();
        }

        public async Task<InventoryTransactionDto?> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _context.InventoryTransactions
                .Include(it => it.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.InventoryTransactionId == transactionId);

            return transaction == null ? null : MapToDto(transaction);
        }

        #endregion

        #region Transaction Operations

        public async Task<InventoryTransactionDto> CreateTransactionAsync(InventoryTransaction transaction)
        {
            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            var createdTransaction = await GetTransactionByIdAsync(transaction.InventoryTransactionId);
            return createdTransaction!;
        }

        #endregion

        #region Stock Operations

        public async Task UpdateStockAsync(int productVariantId, int quantityChange)
        {
            var variant = await _context.ProductVariants.FindAsync(productVariantId);
            if (variant != null)
            {
                variant.StockQuantity += quantityChange;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCurrentStockAsync(int productVariantId)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

            return variant?.StockQuantity ?? 0;
        }

        #endregion

        #region Helper Methods

        private static InventoryTransactionDto MapToDto(InventoryTransaction transaction)
        {
            return new InventoryTransactionDto
            {
                InventoryTransactionId = transaction.InventoryTransactionId,
                ProductVariantId = transaction.ProductVariantId,
                SKU = transaction.ProductVariant?.SKU ?? string.Empty,
                QuantityChanged = transaction.QuantityChanged,
                StockBefore = transaction.StockBefore,
                StockAfter = transaction.StockAfter,
                Type = transaction.Type,
                Note = transaction.Note,
                CreatedAt = transaction.CreatedAt
            };
        }

        #endregion
    }
}
