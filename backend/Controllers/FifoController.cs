using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services.Fifo;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/fifo")]
[Authorize]
public class FifoController : BaseController
{
    private readonly IFifoService _fifoService;

    public FifoController(
        IFifoService fifoService,
        ILogger<FifoController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService)
        : base(logger, configuration, configurationService)
    {
        _fifoService = fifoService;
    }

    [HttpGet("oldest-lot")]
    public async Task<ActionResult<ApiResponse<FifoOldestLotResponse>>> GetOldestLot(
        [FromQuery] string itemCode,
        [FromQuery] string? warehouseCode = null,
        [FromQuery] string? siteReference = null)
    {
        try
        {
            var warehouse = GetDefaultWarehouse(warehouseCode);
            var site = GetDefaultSite(siteReference);
            var response = await _fifoService.GetOldestLotAsync(itemCode, warehouse, site);
            return Ok(ApiResponse<FifoOldestLotResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<FifoOldestLotResponse>(ex, "get oldest lot");
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<FifoValidationResponse>>> Validate([FromBody] FifoValidationRequest request)
    {
        var modelStateError = HandleModelStateErrors<FifoValidationResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var siteReference = GetDefaultSite(request.SiteReference);
            var response = await _fifoService.ValidateFifoComplianceAsync(
                request.ItemCode, request.LotNumber, warehouseCode, siteReference);
            return Ok(ApiResponse<FifoValidationResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<FifoValidationResponse>(ex, "validate FIFO");
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<FifoSummaryResponse>>> GetSummary(
        [FromQuery] string itemCode,
        [FromQuery] string? warehouseCode = null,
        [FromQuery] string? siteReference = null)
    {
        try
        {
            var warehouse = GetDefaultWarehouse(warehouseCode);
            var site = GetDefaultSite(siteReference);
            var response = await _fifoService.GetFifoSummaryAsync(itemCode, warehouse, site);
            return Ok(ApiResponse<FifoSummaryResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<FifoSummaryResponse>(ex, "get FIFO summary");
        }
    }
}

