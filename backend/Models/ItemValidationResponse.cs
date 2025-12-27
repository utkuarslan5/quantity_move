using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class ItemValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("is_lot_tracked")]
    public bool IsLotTracked { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

