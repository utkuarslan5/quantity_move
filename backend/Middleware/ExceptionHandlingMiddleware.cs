using quantity_move_api.Models;
using quantity_move_api.Common;
using System.Net;
using System.Text;
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
            
            // Capture request body if available
            string? requestBody = null;
            if (context.Request.Method != "GET" && context.Request.Method != "HEAD" && context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > 0)
            {
                try
                {
                    context.Request.EnableBuffering();
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    requestBody = body.Length > 1024 ? body.Substring(0, 1024) + "..." : body;
                }
                catch
                {
                    requestBody = "[Unable to read request body]";
                }
            }
            
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            using (LogContext.PushProperty("Username", username))
            using (LogContext.PushProperty("UserId", userId ?? "N/A"))
            using (LogContext.PushProperty("RemoteIpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"))
            {
                // Log exception chain
                _logger.LogExceptionChain(ex, $"UnhandledException: {context.Request.Path}");
                
                // Log business exception with full context
                var exceptionContext = new
                {
                    Path = context.Request.Path.ToString(),
                    Method = context.Request.Method,
                    QueryString = context.Request.QueryString.ToString(),
                    RequestBody = LoggingExtensions.SanitizeParameters(requestBody),
                    Headers = context.Request.Headers.Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(h => h.Key, h => h.Value.ToString())
                };
                
                _logger.LogBusinessException(ex, $"UnhandledException: {context.Request.Path}", exceptionContext);
                
                _logger.LogError(ex, 
                    "An unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}, User: {Username}, IP: {RemoteIpAddress}, ExceptionType: {ExceptionType}",
                    correlationId, 
                    context.Request.Path,
                    context.Request.Method,
                    username,
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    ex.GetType().Name);
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

