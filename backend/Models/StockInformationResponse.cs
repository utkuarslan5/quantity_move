using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockInformationResponse
{
    [JsonPropertyName("items")]
    public List<StockInformationItem> Items { get; set; } = new();
}

public class StockInformationItem
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;
    
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;
    
    [JsonPropertyName("lot_number")]
    public string? LotNumber { get; set; }
    
    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }
    
    [JsonPropertyName("fifo_date")]
    public DateTime? FifoDate { get; set; }
    
    [JsonPropertyName("oldest_lot")]
    public string? OldestLot { get; set; }
    
    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
    
    [JsonPropertyName("site_reference")]
    public string? SiteReference { get; set; }
}

