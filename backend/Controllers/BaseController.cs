using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Common;
using Serilog.Context;

namespace quantity_move_api.Controllers;

/// <summary>
/// Base controller class that provides common functionality for controllers that work with warehouse operations.
/// 
/// Controllers that need warehouse/site default values and common error handling should inherit from this class.
/// Controllers that don't need warehouse context (e.g., AuthController, HealthController, MetricsController)
/// should inherit directly from ControllerBase.
/// 
/// This base class provides:
/// - GetDefaultWarehouse() - Gets default warehouse code from configuration
/// - GetDefaultSite() - Gets default site reference from configuration
/// - HandleModelStateErrors() - Standardized model validation error handling
/// - HandleError() - Standardized exception handling with logging
/// </summary>
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger Logger;
    protected readonly IConfiguration Configuration;
    protected readonly IConfigurationService? ConfigurationService;

    protected BaseController(ILogger logger, IConfiguration configuration, IConfigurationService? configurationService = null)
    {
        Logger = logger;
        Configuration = configuration;
        ConfigurationService = configurationService;
    }

    protected string GetDefaultWarehouse(string? providedWarehouse)
    {
        if (!string.IsNullOrWhiteSpace(providedWarehouse))
            return providedWarehouse;
        
        return ConfigurationService?.GetDefaultWarehouse() ?? Configuration["Defaults:DefaultWarehouse"] ?? "MAIN";
    }

    protected string GetDefaultSite(string? providedSite)
    {
        if (!string.IsNullOrWhiteSpace(providedSite))
            return providedSite;
        
        return ConfigurationService?.GetDefaultSite() ?? Configuration["Defaults:DefaultSite"] ?? "Default";
    }

    protected ActionResult<ApiResponse<T>>? HandleModelStateErrors<T>()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            // Log validation errors with full model state
            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();
            Logger.LogWarning(
                "Model validation failed. Errors: {Errors} | CorrelationId: {CorrelationId} | Path: {Path} | Method: {Method}",
                string.Join("; ", errors),
                correlationId,
                HttpContext.Request.Path,
                HttpContext.Request.Method);
            
            return BadRequest(ApiResponse<T>.ErrorResponse("Invalid request", errors));
        }
        return null;
    }

    protected ActionResult<ApiResponse<T>> HandleError<T>(Exception ex, string operation)
    {
        var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();
        var username = HttpContext.User?.Identity?.Name ?? "Anonymous";
        
        // Create context object for logging
        var errorContext = new
        {
            Operation = operation,
            Path = HttpContext.Request.Path.ToString(),
            Method = HttpContext.Request.Method,
            Username = username,
            QueryString = HttpContext.Request.QueryString.ToString()
        };
        
        // Use enhanced exception logging
        Logger.LogBusinessException(ex, operation, errorContext);
        Logger.LogExceptionChain(ex, operation);
        
        Logger.LogError(ex,
            "Error during {Operation} | CorrelationId: {CorrelationId} | Path: {Path} | Method: {Method} | User: {Username} | ExceptionType: {ExceptionType}",
            operation,
            correlationId,
            HttpContext.Request.Path,
            HttpContext.Request.Method,
            username,
            ex.GetType().Name);
        
        return StatusCode(500, ApiResponse<T>.ErrorResponse($"An error occurred during {operation}"));
    }
}

