using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Data;
using Sales_Management_System_API.DTO.PaymentDtos;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;

namespace Sales_Management_System_API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Payment Queries

        public async Task<List<PaymentDto>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .AsNoTracking()
                .Select(p => MapToPaymentDto(p))
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetByIdAsync(int paymentId)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            return payment == null ? null : MapToPaymentDto(payment);
        }

        public async Task<PaymentDto?> GetByTransactionCodeAsync(string transactionCode)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TransactionCode == transactionCode);

            return payment == null ? null : MapToPaymentDto(payment);
        }

        public async Task<bool> IsTransactionCodeUniqueAsync(string transactionCode)
        {
            return !await _context.Payments
                .AnyAsync(p => p.TransactionCode == transactionCode);
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId && p.Status == "Completed")
                .SumAsync(p => p.Amount);
        }

        #endregion

        #region Payment Operations

        public async Task<PaymentDto> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var createdPayment = await GetByIdAsync(payment.PaymentId);
            return createdPayment!;
        }

        public async Task<PaymentDto> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            var updatedPayment = await GetByIdAsync(payment.PaymentId);
            return updatedPayment!;
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, string status)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Generate Transaction Code

        public async Task<string> GenerateTransactionCodeAsync()
        {
            string transactionCode;
            bool isUnique = false;

            do
            {
                transactionCode = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
                isUnique = await IsTransactionCodeUniqueAsync(transactionCode);
            } while (!isUnique);

            return transactionCode;
        }

        #endregion

        #region Helper Methods

        private static PaymentDto MapToPaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionCode = payment.TransactionCode,
                CreatedAt = payment.CreatedAt
            };
        }

        #endregion
    }
}
