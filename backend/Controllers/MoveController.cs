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
    /// First validates the move, then executes it by calling the TR_Miktar_Ilerlet stored procedure.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> Move([FromBody] MoveQuantityRequest request)
    {
        var modelStateError = HandleModelStateErrors<MoveQuantityResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            // Apply defaults if not provided - create a new object to avoid mutating the input parameter
            var requestWithDefaults = new MoveQuantityRequest
            {
                ItemCode = request.ItemCode,
                SourceLotNumber = request.SourceLotNumber,
                SourceLocation = request.SourceLocation,
                TargetLocation = request.TargetLocation,
                Quantity = request.Quantity,
                WarehouseCode = string.IsNullOrWhiteSpace(request.WarehouseCode) 
                    ? GetDefaultWarehouse(null) 
                    : request.WarehouseCode,
                SiteReference = string.IsNullOrWhiteSpace(request.SiteReference) 
                    ? GetDefaultSite(null) 
                    : request.SiteReference
            };

            // Use MoveQuantityWithFifoCheckAsync to ensure FIFO compliance is checked
            // This method performs FIFO validation, then standard validation, then executes the move
            // Return codes:
            //   -2: FIFO compliance validation failed
            //   -1: Standard validation failed (invalid request parameters)
            //    0: Success
            //   >0: Database stored procedure error
            var response = await _moveService.MoveQuantityWithFifoCheckAsync(requestWithDefaults);

            if (!response.Success)
            {
                // Handle different failure scenarios based on return code
                if (response.ReturnCode == -1)
                {
                    // Validation failure - return 400 Bad Request for client errors
                    return BadRequest(ApiResponse<MoveQuantityResponse>.ErrorResponse(
                        response.ErrorMessage ?? "Move validation failed",
                        new List<string> { response.ErrorMessage ?? "Move validation failed" }));
                }
                else if (response.ReturnCode == -2)
                {
                    // FIFO compliance failure - return 422 Unprocessable Entity for business rule violations
                    return UnprocessableEntity(ApiResponse<MoveQuantityResponse>.ErrorResponse(
                        response.ErrorMessage ?? "FIFO compliance validation failed",
                        new List<string> { response.ErrorMessage ?? "FIFO compliance validation failed" }));
                }
                else
                {
                    // Other operation failures (database errors, etc.) - return 422 Unprocessable Entity
                    return UnprocessableEntity(ApiResponse<MoveQuantityResponse>.ErrorResponse(
                        response.ErrorMessage ?? "Move operation failed",
                        new List<string> { response.ErrorMessage ?? "Move operation failed" }));
                }
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "move quantity");
        }
    }
}

