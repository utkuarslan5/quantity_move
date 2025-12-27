using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockAvailabilityResponse
{
    [JsonPropertyName("is_available")]
    public bool IsAvailable { get; set; }

    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("available_quantity")]
    public decimal AvailableQuantity { get; set; }

    [JsonPropertyName("required_quantity")]
    public decimal RequiredQuantity { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

