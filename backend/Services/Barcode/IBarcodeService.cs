using quantity_move_api.Models;

namespace quantity_move_api.Services.Barcode;

public interface IBarcodeService
{
    // Parse barcode string (item%lot%quantity)
    Task<BarcodeParseResponse> ParseBarcodeAsync(string barcode);

    // Lookup item/lot from barcode
    Task<BarcodeLookupResponse> LookupBarcodeAsync(string barcode);
}

