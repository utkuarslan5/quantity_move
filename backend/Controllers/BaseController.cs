using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

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
            return BadRequest(ApiResponse<T>.ErrorResponse("Invalid request", errors));
        }
        return null;
    }

    protected ActionResult<ApiResponse<T>> HandleError<T>(Exception ex, string operation)
    {
        Logger.LogError(ex, "Error during {Operation}", operation);
        return StatusCode(500, ApiResponse<T>.ErrorResponse($"An error occurred during {operation}"));
    }
}

