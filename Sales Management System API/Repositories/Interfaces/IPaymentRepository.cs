using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.PaymentDtos;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        // Payment queries
        Task<List<PaymentDto>> GetByOrderIdAsync(int orderId);
        Task<PaymentDto?> GetByIdAsync(int paymentId);
        Task<PaymentDto?> GetByTransactionCodeAsync(string transactionCode);
        Task<bool> IsTransactionCodeUniqueAsync(string transactionCode);
        Task<decimal> GetTotalPaidAmountAsync(int orderId);

        // Payment operations
        Task<PaymentDto> CreateAsync(Payment payment);
        Task<PaymentDto> UpdateAsync(Payment payment);
        Task UpdatePaymentStatusAsync(int paymentId, string status);

        // Generate transaction code
        Task<string> GenerateTransactionCodeAsync();
    }
}
