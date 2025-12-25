using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class StockController : BaseController
{
    private readonly IStockService _stockService;

    public StockController(
        IStockService stockService,
        ILogger<StockController> logger,
        IConfiguration configuration)
        : base(logger, configuration)
    {
        _stockService = stockService;
    }

    [HttpPost("lookup")]
    public async Task<ActionResult<ApiResponse<StockLookupResponse>>> Lookup([FromBody] StockLookupRequest request)
    {
        var modelStateError = HandleModelStateErrors<StockLookupResponse>();
        if (modelStateError != null) return modelStateError;

        try
        {
            var response = await _stockService.GetStockAsync(request);
            return Ok(ApiResponse<StockLookupResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<StockLookupResponse>(ex, "stock lookup");
        }
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<StockLookupResponse>>> LookupGet(
        [FromQuery] string? itemCode,
        [FromQuery] string? location,
        [FromQuery] string? warehouse,
        [FromQuery] string? site,
        [FromQuery] string? lot)
    {
        var request = new StockLookupRequest
        {
            ItemCode = itemCode,
            Location = location,
            Warehouse = warehouse,
            Site = site,
            Lot = lot
        };

        return await Lookup(request);
    }
}

