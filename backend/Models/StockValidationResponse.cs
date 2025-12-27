using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }
    
    [JsonPropertyName("return_code")]
    public int ReturnCode { get; set; }
    
    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
    
    [JsonPropertyName("available_quantity")]
    public decimal? AvailableQuantity { get; set; }
    
    [JsonPropertyName("location_info")]  // NEW: If ReturnLocationInfo = true
    public LocationInfo? LocationInfo { get; set; }
}

