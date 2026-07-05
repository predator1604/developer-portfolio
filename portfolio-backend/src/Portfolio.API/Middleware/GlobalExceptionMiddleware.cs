using System.Net;
using System.Text.Json;

namespace Portfolio.API.Middleware;

/// <summary>
/// Global exception handler middleware.
/// Catches any unhandled exception and returns a structured JSON error response.
/// Prevents stack traces leaking to clients in production.
/// </summary>
public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (status, message) = exception switch
        {
            ArgumentException      => (HttpStatusCode.BadRequest,           exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,    "Unauthorized."),
            KeyNotFoundException   => (HttpStatusCode.NotFound,             "Resource not found."),
            _                      => (HttpStatusCode.InternalServerError,  "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)status;

        var response = new
        {
            error  = message,
            code   = status.ToString(),
            detail = env.IsDevelopment() ? exception.StackTrace : null,
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
