using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class QuantityController : BaseController
{
    private readonly IQuantityService _quantityService;

    public QuantityController(
        IQuantityService quantityService,
        ILogger<QuantityController> logger,
        IConfiguration configuration)
        : base(logger, configuration)
    {
        _quantityService = quantityService;
    }

    [HttpPost("move")]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> Move([FromBody] MoveQuantityRequest request)
    {
        var modelStateError = HandleModelStateErrors<MoveQuantityResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var response = await _quantityService.MoveQuantityAsync(request);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, response.Message ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "quantity move");
        }
    }

}

