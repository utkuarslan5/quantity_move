using System.Diagnostics;
using System.Text;
using Serilog.Context;
using quantity_move_api.Services;

namespace quantity_move_api.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with performance metrics
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly IMetricsService? _metricsService;
    private const int MaxBodyLengthBytes = 1024; // 1KB max body length to log
    private const int SlowRequestThresholdMs = 1000; // Requests taking longer than 1 second are considered slow

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IMetricsService? metricsService = null)
    {
        _next = next;
        _logger = logger;
        _metricsService = metricsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
        
        // Enrich log context with request information
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        {
            // Get user information if authenticated
            var username = context.User?.Identity?.Name ?? "Anonymous";
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            using (LogContext.PushProperty("Username", username))
            using (LogContext.PushProperty("UserId", userId ?? "N/A"))
            {
                // Log request
                _logger.LogInformation(
                    "HTTP {Method} {Path} started. User: {Username}, IP: {RemoteIpAddress}",
                    context.Request.Method,
                    context.Request.Path,
                    username,
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

                // Capture request body if needed (for non-GET requests)
                string? requestBody = null;
                if (context.Request.Method != "GET" && context.Request.Method != "HEAD")
                {
                    requestBody = await CaptureRequestBodyAsync(context.Request);
                }

                // Capture original response body stream
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    var duration = stopwatch.ElapsedMilliseconds;

                    // Record metrics
                    if (_metricsService != null)
                    {
                        var endpoint = $"{context.Request.Method} {context.Request.Path}";
                        _metricsService.RecordRequest(endpoint, context.Response.StatusCode, duration);
                        
                        // Track slow requests
                        if (duration > SlowRequestThresholdMs)
                        {
                            _metricsService.IncrementError("SlowRequest");
                        }
                        
                        // Track errors
                        if (context.Response.StatusCode >= 400)
                        {
                            _metricsService.IncrementError($"HttpError_{context.Response.StatusCode}");
                        }
                    }

                    // Log response
                    var responseBodyContent = await CaptureResponseBodyAsync(context.Response);
                    
                    // Determine log level based on status code and duration
                    var logLevel = DetermineLogLevel(context.Response.StatusCode, duration);
                    
                    if (logLevel == LogLevel.Warning || logLevel == LogLevel.Error)
                    {
                        _logger.Log(logLevel,
                            "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms. User: {Username}. RequestBody: {RequestBody}, ResponseBody: {ResponseBody} | ContentLength: {ContentLength} | CorrelationId: {CorrelationId}",
                            context.Request.Method,
                            context.Request.Path,
                            context.Response.StatusCode,
                            duration,
                            username,
                            SanitizeBody(requestBody),
                            SanitizeBody(responseBodyContent),
                            context.Response.ContentLength ?? 0,
                            correlationId);
                    }
                    else
                    {
                        _logger.Log(logLevel,
                            "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms. User: {Username} | ContentLength: {ContentLength} | CorrelationId: {CorrelationId}",
                            context.Request.Method,
                            context.Request.Path,
                            context.Response.StatusCode,
                            duration,
                            username,
                            context.Response.ContentLength ?? 0,
                            correlationId);
                    }

                    // Copy response body back to original stream
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }

    private async Task<string?> CaptureRequestBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body.Length > MaxBodyLengthBytes ? body.Substring(0, MaxBodyLengthBytes) + "..." : body;
    }

    private async Task<string?> CaptureResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return body.Length > MaxBodyLengthBytes ? body.Substring(0, MaxBodyLengthBytes) + "..." : body;
    }

    private LogLevel DetermineLogLevel(int statusCode, long duration)
    {
        // Log errors and slow requests at Warning level
        if (statusCode >= 500)
            return LogLevel.Error;
        if (statusCode >= 400)
            return LogLevel.Warning;
        if (duration > SlowRequestThresholdMs)
            return LogLevel.Warning;
        
        return LogLevel.Information;
    }

    private string? SanitizeBody(string? body)
    {
        if (string.IsNullOrEmpty(body))
            return null;

        // Remove sensitive data patterns (passwords, tokens, etc.)
        // This is a basic implementation - enhance as needed
        var sanitized = body;
        
        // Remove JWT tokens
        sanitized = System.Text.RegularExpressions.Regex.Replace(
            sanitized,
            @"Bearer\s+[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+",
            "Bearer [REDACTED]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Remove password fields
        sanitized = System.Text.RegularExpressions.Regex.Replace(
            sanitized,
            @"""password""\s*:\s*""[^""]*""",
            "\"password\":\"[REDACTED]\"",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return sanitized;
    }
}

