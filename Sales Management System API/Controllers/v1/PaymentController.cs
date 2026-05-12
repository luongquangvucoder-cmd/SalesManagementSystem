using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales_Management_System_API.DTO;
using Sales_Management_System_API.DTO.PaymentDtos;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _paymentService.GetByOrderIdAsync(orderId);
            return Ok(new ApiResponse<List<PaymentDto>>
            {
                Success = true,
                Message = "Payments retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment retrieved successfully",
                Data = result
            });
        }

        [HttpGet("code/{transactionCode}")]
        public async Task<IActionResult> GetByTransactionCode(string transactionCode)
        {
            var result = await _paymentService.GetByTransactionCodeAsync(transactionCode);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.CreatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.PaymentId },
                new ApiResponse<PaymentDto>
                {
                    Success = true,
                    Message = "Payment created successfully",
                    Data = result
                });
        }

        //[HttpPut("{id}/status")]
        ////[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _paymentService.UpdatePaymentStatusAsync(id, dto);
        //    return Ok(new ApiResponse<PaymentDto>
        //    {
        //        Success = true,
        //        Message = "Payment status updated successfully",
        //        Data = result
        //    });
        //}

        [HttpPut("{id}/complete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompletePayment(int id)
        {
            var result = await _paymentService.CompletePaymentAsync(id);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment completed successfully",
                Data = result
            });
        }

        [HttpPut("{id}/fail")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> FailPayment(int id)
        {
            var result = await _paymentService.FailPaymentAsync(id);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment marked as failed",
                Data = result
            });
        }

        [HttpPut("{id}/refund")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefundPayment(int id)
        {
            var result = await _paymentService.RefundPaymentAsync(id);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment refunded successfully",
                Data = result
            });
        }
    }
}
