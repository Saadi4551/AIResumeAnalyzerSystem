using System.Net;
using System.Text.Json;

namespace AIResumeAnalyzerSystem.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            // ✅ 400 - Validation errors (email already exists, invalid input)
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),

            // ✅ 401 - Login required
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Invalid email or password."),

            // ✅ 404 - Resource not found
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),

            // ✅ 429 - Analysis limit reached
            InvalidOperationException when exception.Message.Contains("limit") =>
                (HttpStatusCode.TooManyRequests, exception.Message),

            // ✅ 500 - All other errors
            _ => (HttpStatusCode.InternalServerError, "Something went wrong. Please try again.")
        };

        context.Response.StatusCode = (int)statusCode;

        // ✅ Consistent response format - same as success responses
        var response = new
        {
            success = false,
            message = message,
            data = (object?)null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}