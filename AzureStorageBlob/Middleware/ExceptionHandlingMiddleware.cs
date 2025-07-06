using System.Net;

namespace AzureStorageBlob.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured {context.Request.Path}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                Message = "An exception error occurred mdl.",
                Details = ex.Message.ToString(),
                ExceptionType = ex.GetType().Name,
                StackTrace = env.IsDevelopment() ? ex.StackTrace : null // Return class name, method name & line number
                // Returning stack trace is not good idea on production server.
                // Instead of returning stack trace, you can add it in log
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
