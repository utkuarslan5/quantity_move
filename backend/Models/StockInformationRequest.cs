using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockInformationRequest
{
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; set; }
    
    [JsonPropertyName("lot_number")]
    public string? LotNumber { get; set; }
    
    [JsonPropertyName("location_code")]
    public string? LocationCode { get; set; }
    
    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
    
    [JsonPropertyName("site_reference")]
    public string? SiteReference { get; set; }
    
    [JsonPropertyName("sort_by")]
    public string? SortBy { get; set; }  // "fifo", "quantity", "location"
    
    [JsonPropertyName("include_fifo_validation")]
    public bool IncludeFifoValidation { get; set; } = true;
}

