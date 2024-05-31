using Exceptions;
using System.Net;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                DuplicateTariffException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            var message = exception switch
            {
                DuplicateTariffException duplicateException => duplicateException.Message,
                TariffIdMismatchException idMismatchException => idMismatchException.Message,
                KeyNotFoundException keyNotFoundException => keyNotFoundException.Message,
                _ => "Internal Server Error from the custom middleware."
            };

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
