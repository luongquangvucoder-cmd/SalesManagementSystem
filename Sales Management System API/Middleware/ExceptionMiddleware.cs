using Sales_Management_System_API.Exceptions;
using System.Net;
using System.Text.Json;

namespace Sales_Management_System_API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    BadRequestException => HttpStatusCode.BadRequest,
                    UnauthorizedException => HttpStatusCode.Unauthorized,
                    _ => HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    Success = false,
                    Message = ex.Message
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
        }
    }
}
