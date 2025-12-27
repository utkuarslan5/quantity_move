using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LocationValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("location_type")]
    public string? LocationType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

