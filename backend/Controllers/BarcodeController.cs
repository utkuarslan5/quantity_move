using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services.Barcode;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/barcode")]
[Authorize]
public class BarcodeController : BaseController
{
    private readonly IBarcodeService _barcodeService;

    public BarcodeController(
        IBarcodeService barcodeService,
        ILogger<BarcodeController> logger,
        IConfiguration configuration)
        : base(logger, configuration)
    {
        _barcodeService = barcodeService;
    }

    [HttpPost("parse")]
    public async Task<ActionResult<ApiResponse<BarcodeParseResponse>>> Parse([FromBody] string barcode)
    {
        try
        {
            var response = await _barcodeService.ParseBarcodeAsync(barcode);
            return Ok(ApiResponse<BarcodeParseResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<BarcodeParseResponse>(ex, "parse barcode");
        }
    }

    [HttpPost("lookup")]
    public async Task<ActionResult<ApiResponse<BarcodeLookupResponse>>> Lookup([FromBody] string barcode)
    {
        try
        {
            var response = await _barcodeService.LookupBarcodeAsync(barcode);
            return Ok(ApiResponse<BarcodeLookupResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return HandleError<BarcodeLookupResponse>(ex, "lookup barcode");
        }
    }
}

