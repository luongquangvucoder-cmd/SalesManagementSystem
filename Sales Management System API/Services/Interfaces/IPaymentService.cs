using Sales_Management_System_API.DTO.PaymentDtos;

namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IPaymentService
    {
        // Payment queries
        Task<List<PaymentDto>> GetByOrderIdAsync(int orderId);
        Task<PaymentDto> GetByIdAsync(int paymentId);
        Task<PaymentDto> GetByTransactionCodeAsync(string transactionCode);
        Task<decimal> GetTotalPaidAmountAsync(int orderId);

        // Payment operations
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);
        Task<PaymentDto> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusDto dto);
        Task<PaymentDto> CompletePaymentAsync(int paymentId);
        Task<PaymentDto> FailPaymentAsync(int paymentId);
        Task<PaymentDto> RefundPaymentAsync(int paymentId);
    }
}
