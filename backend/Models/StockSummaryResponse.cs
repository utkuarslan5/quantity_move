using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockSummaryResponse
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("warehouse_code")]
    public string WarehouseCode { get; set; } = string.Empty;

    [JsonPropertyName("total_quantity")]
    public decimal TotalQuantity { get; set; }

    [JsonPropertyName("location_count")]
    public int LocationCount { get; set; }

    [JsonPropertyName("locations")]
    public List<StockSummaryLocation> Locations { get; set; } = new();
}

public class StockSummaryLocation
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }
}

