using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/move")]
[Authorize]
public class MoveController : BaseController
{
    private readonly IQuantityMoveService _moveService;
    private readonly IQuantityValidationService _validationService;

    public MoveController(
        IQuantityMoveService moveService,
        IQuantityValidationService validationService,
        ILogger<MoveController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService)
        : base(logger, configuration, configurationService)
    {
        _moveService = moveService;
        _validationService = validationService;
    }

    /// <summary>
    /// Validate if a move operation is allowed.
    /// Checks that source location has sufficient quantity and target location is valid.
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<MoveValidationResponse>>> ValidateMove([FromBody] MoveValidationRequest request)
    {
        var modelStateError = HandleModelStateErrors<MoveValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var requestWithDefaults = new MoveValidationRequest
            {
                ItemCode = request.ItemCode,
                LotNumber = request.LotNumber,
                SourceLocation = request.SourceLocation,
                TargetLocation = request.TargetLocation,
                Quantity = request.Quantity,
                WarehouseCode = warehouseCode
            };

            var response = await _validationService.ValidateMoveAsync(requestWithDefaults);
            return Ok(ApiResponse<MoveValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<MoveValidationResponse>(ex, "validate move");
        }
    }

    /// <summary>
    /// Execute a move operation.
    /// First validates the move, then executes it by calling the move_quantity stored procedure.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> Move([FromBody] MoveQuantityRequest request)
    {
        var modelStateError = HandleModelStateErrors<MoveQuantityResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            // Apply defaults if not provided
            if (string.IsNullOrWhiteSpace(request.WarehouseCode))
                request.WarehouseCode = GetDefaultWarehouse(null);
            if (string.IsNullOrWhiteSpace(request.SiteReference))
                request.SiteReference = GetDefaultSite(null);

            // First validate the move
            var validationRequest = new MoveValidationRequest
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                SourceLocation = request.SourceLocation,
                TargetLocation = request.TargetLocation,
                Quantity = request.Quantity,
                WarehouseCode = request.WarehouseCode
            };

            var validation = await _validationService.ValidateMoveAsync(validationRequest);
            if (!validation.IsValid)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.ErrorResponse(
                    validation.ErrorMessage ?? "Move validation failed",
                    new List<string> { validation.ErrorMessage ?? "Move validation failed" }));
            }

            // Proceed with move
            var response = await _moveService.MoveQuantityAsync(request);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(
                    response, 
                    response.ErrorMessage ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "move quantity");
        }
    }
}

