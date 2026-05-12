using Sales_Management_System_API.DTO.PaymentDtos;
using Sales_Management_System_API.Exceptions;
using Sales_Management_System_API.Models;
using Sales_Management_System_API.Repositories.Interfaces;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        #region Payment Queries

        public async Task<List<PaymentDto>> GetByOrderIdAsync(int orderId)
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            return await _paymentRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<PaymentDto> GetByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            return payment;
        }

        public async Task<PaymentDto> GetByTransactionCodeAsync(string transactionCode)
        {
            var payment = await _paymentRepository.GetByTransactionCodeAsync(transactionCode);
            if (payment == null)
                throw new NotFoundException($"Payment with transaction code {transactionCode} not found");

            return payment;
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with id {orderId} not found");

            return await _paymentRepository.GetTotalPaidAmountAsync(orderId);
        }

        #endregion

        #region Payment Operations

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
                throw new BadRequestException($"Order with id {dto.OrderId} not found");

            // Validate payment method
            var validMethods = PaymentMethodConstants.GetAllMethods();
            if (!validMethods.Contains(dto.PaymentMethod))
                throw new BadRequestException($"Invalid payment method. Valid methods: {string.Join(", ", validMethods)}");

            // Validate amount
            if (dto.Amount <= 0)
                throw new BadRequestException("Payment amount must be greater than 0");

            // Get total paid amount
            var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(dto.OrderId);
            var remainingAmount = order.TotalAmount - totalPaid;

            if (dto.Amount > remainingAmount)
                throw new BadRequestException($"Payment amount exceeds remaining balance. Remaining: {remainingAmount}");

            // Generate transaction code if not provided
            var transactionCode = dto.TransactionCode;
            if (string.IsNullOrWhiteSpace(transactionCode))
            {
                transactionCode = await _paymentRepository.GenerateTransactionCodeAsync();
            }
            else
            {
                // Validate transaction code is unique
                var isUnique = await _paymentRepository.IsTransactionCodeUniqueAsync(transactionCode);
                if (!isUnique)
                    throw new ConflictException("Transaction code already exists");
            }

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = PaymentStatusConstants.Pending,
                TransactionCode = transactionCode,
                CreatedAt = DateTime.UtcNow
            };

            return await _paymentRepository.CreateAsync(payment);
        }

        public async Task<PaymentDto> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            // Validate status
            var validStatuses = PaymentStatusConstants.GetAllStatuses();
            if (!validStatuses.Contains(dto.Status))
                throw new BadRequestException($"Invalid payment status. Valid statuses: {string.Join(", ", validStatuses)}");

            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, dto.Status);
            return await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new BadRequestException("Failed to retrieve updated payment");
        }

        public async Task<PaymentDto> CompletePaymentAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            if (payment.Status == PaymentStatusConstants.Completed)
                throw new BadRequestException("Payment is already completed");

            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, PaymentStatusConstants.Completed);

            // Check if order is fully paid and update order status
            var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(payment.OrderId);
            var order = await _orderRepository.GetOrderEntityByIdAsync(payment.OrderId);

            if (order != null && totalPaid >= order.TotalAmount)
            {
                order.PaymentStatus = "Paid";
                await _orderRepository.UpdateAsync(order);
            }

            return await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new BadRequestException("Failed to retrieve completed payment");
        }

        public async Task<PaymentDto> FailPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            if (payment.Status == PaymentStatusConstants.Failed)
                throw new BadRequestException("Payment is already marked as failed");

            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, PaymentStatusConstants.Failed);
            return await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new BadRequestException("Failed to retrieve failed payment");
        }

        public async Task<PaymentDto> RefundPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            if (payment.Status != PaymentStatusConstants.Completed)
                throw new BadRequestException("Only completed payments can be refunded");

            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, PaymentStatusConstants.Refunded);
            return await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new BadRequestException("Failed to retrieve refunded payment");
        }

        #endregion
    }
}
