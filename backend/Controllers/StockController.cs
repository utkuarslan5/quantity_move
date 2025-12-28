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

    public StockController(
        IStockQueryService queryService,
        ILogger<StockController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService)
        : base(logger, configuration, configurationService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Get stock locations and quantities by barcode.
    /// Barcode format: item_code%lot_num (e.g., "ITEM123%LOT456")
    /// </summary>
    [HttpGet("{barcode}")]
    public async Task<ActionResult<ApiResponse<LocationsResponse>>> GetStockByBarcode([FromRoute] string barcode)
    {
        try
        {
            // Parse barcode: item_code%lot_num
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return BadRequest(ApiResponse<LocationsResponse>.ErrorResponse("Barcode is required"));
            }

            var parts = barcode.Split('%');
            if (parts.Length != 2)
            {
                return BadRequest(ApiResponse<LocationsResponse>.ErrorResponse("Invalid barcode format. Expected format: item_code%lot_num"));
            }

            var itemCode = parts[0].Trim();
            var lotNumber = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(itemCode) || string.IsNullOrWhiteSpace(lotNumber))
            {
                return BadRequest(ApiResponse<LocationsResponse>.ErrorResponse("Invalid barcode format. item_code and lot_num are required"));
            }

            var warehouseCode = GetDefaultWarehouse(null);
            var response = await _queryService.GetLocationsForItemLotAsync(itemCode, lotNumber, warehouseCode, includeZeroQuantity: false);
            
            return Ok(ApiResponse<LocationsResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<LocationsResponse>(ex, "get stock by barcode");
        }
    }
}
