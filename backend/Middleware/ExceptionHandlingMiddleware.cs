using quantity_move_api.Models;
using System.Net;
using System.Text.Json;
using Serilog.Context;

namespace quantity_move_api.Middleware;

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
        // Generate correlation ID if not present
        if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
        {
            context.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
        }
        
        var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Enrich log context with request details
            var username = context.User?.Identity?.Name ?? "Anonymous";
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            using (LogContext.PushProperty("Username", username))
            using (LogContext.PushProperty("UserId", userId ?? "N/A"))
            using (LogContext.PushProperty("RemoteIpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"))
            {
                _logger.LogError(ex, 
                    "An unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}, User: {Username}, IP: {RemoteIpAddress}",
                    correlationId, 
                    context.Request.Path,
                    context.Request.Method,
                    username,
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
            }
            
            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";

        // Handle specific exception types
        if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            message = "Unauthorized access.";
        }
        else if (exception is ArgumentException || exception is ArgumentNullException)
        {
            code = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        else if (exception is KeyNotFoundException)
        {
            code = HttpStatusCode.NotFound;
            message = exception.Message;
        }

        var errors = new List<string> { exception.Message };
        if (exception.InnerException != null)
        {
            errors.Add($"Inner exception: {exception.InnerException.Message}");
        }

        var response = ApiResponse<object>.ErrorResponse(message, errors);

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        return context.Response.WriteAsync(result);
    }
}

