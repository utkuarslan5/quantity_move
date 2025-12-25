using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LocationController : BaseController
{
    private readonly ILocationService _locationService;

    public LocationController(
        ILocationService locationService,
        ILogger<LocationController> logger,
        IConfiguration configuration)
        : base(logger, configuration)
    {
        _locationService = locationService;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<ValidateLocationResponse>>> Validate([FromBody] ValidateLocationRequest request)
    {
        var modelStateError = HandleModelStateErrors<ValidateLocationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var response = await _locationService.ValidateLocationAsync(request);

            if (!response.IsValid)
            {
                return NotFound(ApiResponse<ValidateLocationResponse>.SuccessResponse(response, "Location not found"));
            }

            return Ok(ApiResponse<ValidateLocationResponse>.SuccessResponse(response, "Location validated successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<ValidateLocationResponse>(ex, "location validation");
        }
    }
}

