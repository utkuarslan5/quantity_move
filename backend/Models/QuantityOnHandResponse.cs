using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class QuantityOnHandResponse
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;
    
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;
    
    [JsonPropertyName("lot_number")]
    public string? LotNumber { get; set; }
    
    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }
}

