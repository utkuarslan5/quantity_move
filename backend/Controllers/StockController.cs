using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services.Stock;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController : BaseController
{
    private readonly IStockQueryService _queryService;
    private readonly IStockValidationService _validationService;
    private readonly IStockLocationService _locationService;

    public StockController(
        IStockQueryService queryService,
        IStockValidationService validationService,
        IStockLocationService locationService,
        ILogger<StockController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService)
        : base(logger, configuration, configurationService)
    {
        _queryService = queryService;
        _validationService = validationService;
        _locationService = locationService;
    }

    // Query Endpoints
    [HttpGet("quantity")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetQuantity([FromQuery] GetQuantityRequest request)
    {
        var modelStateError = HandleModelStateErrors<decimal>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var quantity = await _queryService.GetQuantityAtLocationAsync(
                request.ItemCode, request.LotNumber, request.LocationCode, warehouseCode);
            return Ok(ApiResponse<decimal>.SuccessResponse(quantity));
        }
        catch (Exception ex)
        {
            return HandleError<decimal>(ex, "get quantity");
        }
    }

    [HttpGet("locations")]
    public async Task<ActionResult<ApiResponse<LocationsResponse>>> GetLocations([FromQuery] GetLocationsRequest request)
    {
        var modelStateError = HandleModelStateErrors<LocationsResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _queryService.GetLocationsForItemLotAsync(
                request.ItemCode, request.LotNumber, warehouseCode, request.IncludeZeroQuantity);
            return Ok(ApiResponse<LocationsResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<LocationsResponse>(ex, "get locations");
        }
    }

    [HttpGet("locations/with-stock")]
    public async Task<ActionResult<ApiResponse<LocationsWithStockResponse>>> GetLocationsWithStock([FromQuery] GetLocationsRequest request)
    {
        var modelStateError = HandleModelStateErrors<LocationsWithStockResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _locationService.GetLocationsWithStockAsync(
                request.ItemCode, request.LotNumber, warehouseCode);
            return Ok(ApiResponse<LocationsWithStockResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<LocationsWithStockResponse>(ex, "get locations with stock");
        }
    }

    [HttpGet("current-location")]
    public async Task<ActionResult<ApiResponse<CurrentLocationResponse>>> GetCurrentLocation([FromQuery] GetCurrentLocationRequest request)
    {
        var modelStateError = HandleModelStateErrors<CurrentLocationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _queryService.GetCurrentLocationAsync(
                request.ItemCode, request.LotNumber, warehouseCode);
            return Ok(ApiResponse<CurrentLocationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<CurrentLocationResponse>(ex, "get current location");
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<StockSummaryResponse>>> GetSummary(
        [FromQuery] string itemCode,
        [FromQuery] string? warehouseCode = null)
    {
        try
        {
            var warehouse = GetDefaultWarehouse(warehouseCode);
            var response = await _queryService.GetStockSummaryAsync(itemCode, warehouse);
            return Ok(ApiResponse<StockSummaryResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<StockSummaryResponse>(ex, "get stock summary");
        }
    }

    // Validation Endpoints
    [HttpPost("validate/item")]
    public async Task<ActionResult<ApiResponse<ItemValidationResponse>>> ValidateItem([FromBody] ValidateItemRequest request)
    {
        var modelStateError = HandleModelStateErrors<ItemValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var siteReference = GetDefaultSite(request.SiteReference);
            var response = await _validationService.ValidateItemAsync(request.ItemCode, siteReference);
            return Ok(ApiResponse<ItemValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<ItemValidationResponse>(ex, "validate item");
        }
    }

    [HttpPost("validate/lot")]
    public async Task<ActionResult<ApiResponse<LotValidationResponse>>> ValidateLot([FromBody] ValidateLotRequest request)
    {
        var modelStateError = HandleModelStateErrors<LotValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var response = await _validationService.ValidateLotAsync(request.ItemCode, request.LotNumber);
            return Ok(ApiResponse<LotValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<LotValidationResponse>(ex, "validate lot");
        }
    }

    [HttpPost("validate/location")]
    public async Task<ActionResult<ApiResponse<LocationValidationResponse>>> ValidateLocation([FromBody] ValidateLocationRequest request)
    {
        var modelStateError = HandleModelStateErrors<LocationValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var siteReference = GetDefaultSite(request.SiteReference);
            var response = await _validationService.ValidateLocationAsync(request.LocationCode, siteReference);
            return Ok(ApiResponse<LocationValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<LocationValidationResponse>(ex, "validate location");
        }
    }

    [HttpPost("validate/availability")]
    public async Task<ActionResult<ApiResponse<StockAvailabilityResponse>>> ValidateAvailability([FromBody] ValidateStockRequest request)
    {
        var modelStateError = HandleModelStateErrors<StockAvailabilityResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var response = await _validationService.ValidateStockAvailabilityAsync(
                request.ItemCode, request.LotNumber, request.LocationCode, 
                request.RequiredQuantity, warehouseCode);
            return Ok(ApiResponse<StockAvailabilityResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<StockAvailabilityResponse>(ex, "validate availability");
        }
    }

    [HttpPost("validate/move")]
    public async Task<ActionResult<ApiResponse<CombinedValidationResponse>>> ValidateForMove([FromBody] StockMoveValidationRequest request)
    {
        var modelStateError = HandleModelStateErrors<CombinedValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            // Apply defaults if not provided
            if (string.IsNullOrWhiteSpace(request.WarehouseCode))
                request.WarehouseCode = GetDefaultWarehouse(null);
            if (string.IsNullOrWhiteSpace(request.SiteReference))
                request.SiteReference = GetDefaultSite(null);
            
            var response = await _validationService.ValidateStockForMoveAsync(request);
            return Ok(ApiResponse<CombinedValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<CombinedValidationResponse>(ex, "validate for move");
        }
    }
}

