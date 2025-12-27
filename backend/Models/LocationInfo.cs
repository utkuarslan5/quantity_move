using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LocationInfo
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;
    
    [JsonPropertyName("location_type")]
    public string LocationType { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

