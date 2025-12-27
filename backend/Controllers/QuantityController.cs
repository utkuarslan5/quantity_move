using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/quantity")]
[Authorize]
public class QuantityController : BaseController
{
    private readonly IQuantityMoveService _moveService;
    private readonly IQuantityValidationService _validationService;

    public QuantityController(
        IQuantityMoveService moveService,
        IQuantityValidationService validationService,
        ILogger<QuantityController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService)
        : base(logger, configuration, configurationService)
    {
        _moveService = moveService;
        _validationService = validationService;
    }

    // Validation Endpoints (Pre-Move)
    [HttpPost("validate/source")]
    public async Task<ActionResult<ApiResponse<ValidationResponse>>> ValidateSource([FromBody] ValidateSourceRequest request)
    {
        var modelStateError = HandleModelStateErrors<ValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _validationService.ValidateSourceStockAsync(
                request.ItemCode, request.LotNumber, request.SourceLocation, 
                request.Quantity, warehouseCode);
            return Ok(ApiResponse<ValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<ValidationResponse>(ex, "validate source");
        }
    }

    [HttpPost("validate/target")]
    public async Task<ActionResult<ApiResponse<ValidationResponse>>> ValidateTarget([FromBody] ValidateTargetRequest request)
    {
        var modelStateError = HandleModelStateErrors<ValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _validationService.ValidateTargetLocationAsync(
                request.ItemCode, request.TargetLocation, warehouseCode);
            return Ok(ApiResponse<ValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<ValidationResponse>(ex, "validate target");
        }
    }

    [HttpPost("validate/move")]
    public async Task<ActionResult<ApiResponse<MoveValidationResponse>>> ValidateMove([FromBody] MoveValidationRequest request)
    {
        var modelStateError = HandleModelStateErrors<MoveValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            // Create a new request with default warehouse applied
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

    // Move Endpoints
    [HttpPost("move")]
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
            
            var response = await _moveService.MoveQuantityAsync(request);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, response.ErrorMessage ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "quantity move");
        }
    }

    [HttpPost("move/with-validation")]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> MoveWithValidation(
        [FromBody] MoveQuantityRequest request)
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
            
            var response = await _moveService.MoveQuantityWithValidationAsync(request);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, response.ErrorMessage ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "quantity move with validation");
        }
    }

    [HttpPost("move/with-fifo")]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> MoveWithFifo(
        [FromBody] MoveQuantityRequest request)
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
            
            var response = await _moveService.MoveQuantityWithFifoCheckAsync(request);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, response.ErrorMessage ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleError<MoveQuantityResponse>(ex, "quantity move with FIFO");
        }
    }
}

