using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class CurrentLocationResponse
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("locations")]
    public List<CurrentLocationItem> Locations { get; set; } = new();
}

public class CurrentLocationItem
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }
}

