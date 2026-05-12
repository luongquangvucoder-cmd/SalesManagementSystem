using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.InventoryDtos;
using Sales_Management_System_API.DTO.ProductDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
        }

        #region Transaction Queries

        public async Task<PagedResult<InventoryTransactionDto>> GetAllTransactionsAsync(InventoryQueryDto query)
        {
            if (query.Page < 1)
                throw new BadRequestException("Page must be greater than 0");

            if (query.PageSize < 1 || query.PageSize > 100)
                throw new BadRequestException("PageSize must be between 1 and 100");

            return await _inventoryRepository.GetAllTransactionsAsync(query);
        }

        public async Task<List<InventoryTransactionDto>> GetTransactionsByVariantAsync(int productVariantId)
        {
            var variant = await _productRepository.GetVariantByIdAsync(productVariantId);
            if (variant == null)
                throw new NotFoundException($"Product variant with id {productVariantId} not found");

            return await _inventoryRepository.GetByProductVariantIdAsync(productVariantId);
        }

        public async Task<InventoryTransactionDto> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _inventoryRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new NotFoundException($"Inventory transaction with id {transactionId} not found");

            return transaction;
        }

        #endregion

        #region Transaction Operations

        public async Task<InventoryTransactionDto> CreateTransactionAsync(CreateInventoryTransactionDto dto)
        {
            // Validate variant exists
            var variant = await _productRepository.GetVariantByIdAsync(dto.ProductVariantId);
            if (variant == null)
                throw new BadRequestException($"Product variant with id {dto.ProductVariantId} not found");

            // Validate transaction type
            var validTypes = InventoryTypeConstants.GetAllTypes();
            if (!validTypes.Contains(dto.Type))
                throw new BadRequestException($"Invalid transaction type. Valid types: {string.Join(", ", validTypes)}");

            // Get current stock
            var currentStock = await _inventoryRepository.GetCurrentStockAsync(dto.ProductVariantId);

            // Validate stock for export
            if ((dto.Type == InventoryTypeConstants.Export || dto.Type == InventoryTypeConstants.Return) &&
                currentStock < Math.Abs(dto.QuantityChanged))
                throw new BadRequestException($"Insufficient stock. Current: {currentStock}, Requested: {Math.Abs(dto.QuantityChanged)}");

            var stockBefore = currentStock;
            var stockAfter = stockBefore + dto.QuantityChanged;

            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                QuantityChanged = dto.QuantityChanged,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                Type = dto.Type,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            // Update stock
            await _inventoryRepository.UpdateStockAsync(dto.ProductVariantId, dto.QuantityChanged);

            return await _inventoryRepository.CreateTransactionAsync(transaction);
        }

        public async Task<InventoryTransactionDto> ImportStockAsync(int productVariantId, int quantity, string? note = null)
        {
            if (quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0");

            var dto = new CreateInventoryTransactionDto
            {
                ProductVariantId = productVariantId,
                QuantityChanged = quantity,
                Type = InventoryTypeConstants.Import,
                Note = note
            };

            return await CreateTransactionAsync(dto);
        }

        public async Task<InventoryTransactionDto> ExportStockAsync(int productVariantId, int quantity, string? note = null)
        {
            if (quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0");

            var dto = new CreateInventoryTransactionDto
            {
                ProductVariantId = productVariantId,
                QuantityChanged = -quantity,
                Type = InventoryTypeConstants.Export,
                Note = note
            };

            return await CreateTransactionAsync(dto);
        }

        public async Task<InventoryTransactionDto> AdjustStockAsync(int productVariantId, int quantity, string? note = null)
        {
            // Quantity can be positive or negative for adjustments
            var dto = new CreateInventoryTransactionDto
            {
                ProductVariantId = productVariantId,
                QuantityChanged = quantity,
                Type = InventoryTypeConstants.Adjustment,
                Note = note ?? "Manual adjustment"
            };

            return await CreateTransactionAsync(dto);
        }

        public async Task<InventoryTransactionDto> ProcessReturnAsync(int productVariantId, int quantity, string? note = null)
        {
            if (quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0");

            var dto = new CreateInventoryTransactionDto
            {
                ProductVariantId = productVariantId,
                QuantityChanged = quantity,
                Type = InventoryTypeConstants.Return,
                Note = note ?? "Customer return"
            };

            return await CreateTransactionAsync(dto);
        }

        #endregion

        #region Stock Queries

        public async Task<int> GetCurrentStockAsync(int productVariantId)
        {
            var variant = await _productRepository.GetVariantByIdAsync(productVariantId);
            if (variant == null)
                throw new NotFoundException($"Product variant with id {productVariantId} not found");

            return await _inventoryRepository.GetCurrentStockAsync(productVariantId);
        }

        public async Task<List<StockLevelDto>> GetLowStockProductsAsync(int minimumThreshold = 10)
        {
            if (minimumThreshold < 0)
                throw new BadRequestException("Minimum threshold must be greater than or equal to 0");

            // Get all variants with stock <= minimumThreshold
            var query = new ProductQueryDto { PageSize = 1000 };
            var allProducts = await _productRepository.GetAllAsync(query);

            var lowStockItems = new List<StockLevelDto>();

            foreach (var product in allProducts.Items)
            {
                foreach (var variant in product.Variants)
                {
                    if (variant.StockQuantity <= minimumThreshold)
                    {
                        lowStockItems.Add(new StockLevelDto
                        {
                            ProductVariantId = variant.ProductVariantId,
                            SKU = variant.SKU,
                            ProductName = product.Name,
                            CurrentStock = variant.StockQuantity,
                            MinimumStock = minimumThreshold
                        });
                    }
                }
            }

            return lowStockItems.OrderBy(x => x.CurrentStock).ToList();
        }

        #endregion
    }
}
