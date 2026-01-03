using System.Text.Json;
using System.Text.RegularExpressions;

namespace quantity_move_api.Common;

/// <summary>
/// Extension methods for enhanced structured logging
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs a business exception with full context
    /// </summary>
    public static void LogBusinessException(this ILogger logger, Exception ex, string operation, object? context = null)
    {
        var contextJson = context != null ? JsonSerializer.Serialize(context) : null;
        logger.LogError(ex,
            "BusinessException in {Operation} | ExceptionType: {ExceptionType} | Message: {Message} | Context: {Context}",
            operation,
            ex.GetType().Name,
            ex.Message,
            contextJson);
    }

    /// <summary>
    /// Logs a database exception with connection info (sanitized)
    /// </summary>
    public static void LogDatabaseException(this ILogger logger, Exception ex, string operation, string? connectionString = null)
    {
        var connectionHash = connectionString != null ? HashConnectionString(connectionString) : "N/A";
        logger.LogError(ex,
            "DatabaseException in {Operation} | ExceptionType: {ExceptionType} | ConnectionHash: {ConnectionHash} | Message: {Message}",
            operation,
            ex.GetType().Name,
            connectionHash,
            ex.Message);
    }

    /// <summary>
    /// Logs exception chain including inner exceptions
    /// </summary>
    public static void LogExceptionChain(this ILogger logger, Exception ex, string operation)
    {
        var exceptionChain = new List<string> { ex.GetType().Name + ": " + ex.Message };
        var innerEx = ex.InnerException;
        var depth = 0;
        
        while (innerEx != null && depth < 5) // Limit depth to prevent infinite loops
        {
            exceptionChain.Add($"InnerException[{depth}]: {innerEx.GetType().Name} - {innerEx.Message}");
            innerEx = innerEx.InnerException;
            depth++;
        }

        logger.LogError(ex,
            "ExceptionChain in {Operation} | Chain: {ExceptionChain}",
            operation,
            string.Join(" | ", exceptionChain));
    }

    /// <summary>
    /// Sanitizes parameters for logging (removes sensitive data)
    /// </summary>
    public static string? SanitizeParameters(object? parameters)
    {
        if (parameters == null)
            return null;

        try
        {
            var json = JsonSerializer.Serialize(parameters);
            
            // Remove password fields
            json = Regex.Replace(json,
                @"""password""\s*:\s*""[^""]*""",
                "\"password\":\"[REDACTED]\"",
                RegexOptions.IgnoreCase);

            // Remove connection strings
            json = Regex.Replace(json,
                @"""connectionString""\s*:\s*""[^""]*""",
                "\"connectionString\":\"[REDACTED]\"",
                RegexOptions.IgnoreCase);

            // Remove JWT tokens
            json = Regex.Replace(json,
                @"Bearer\s+[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+",
                "Bearer [REDACTED]",
                RegexOptions.IgnoreCase);

            return json;
        }
        catch
        {
            return "[Unable to serialize parameters]";
        }
    }

    /// <summary>
    /// Hashes connection string for logging (doesn't expose sensitive data)
    /// </summary>
    private static string HashConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "N/A";

        try
        {
            // Extract just the server and database name for identification
            var serverMatch = Regex.Match(connectionString, @"Data Source=([^;]+)", RegexOptions.IgnoreCase);
            var databaseMatch = Regex.Match(connectionString, @"Initial Catalog=([^;]+)", RegexOptions.IgnoreCase);
            
            var server = serverMatch.Success ? serverMatch.Groups[1].Value : "Unknown";
            var database = databaseMatch.Success ? databaseMatch.Groups[1].Value : "Unknown";
            
            return $"{server}/{database}";
        }
        catch
        {
            return "HashError";
        }
    }
}

