using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;

namespace quantity_move_api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly ILogger Logger;
    protected readonly IConfiguration Configuration;

    protected BaseController(ILogger logger, IConfiguration configuration)
    {
        Logger = logger;
        Configuration = configuration;
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

