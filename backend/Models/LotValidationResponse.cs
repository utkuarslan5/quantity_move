using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LotValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

