using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class FifoValidationResponse
{
    [JsonPropertyName("is_compliant")]
    public bool IsCompliant { get; set; }

    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("current_lot_number")]
    public string CurrentLotNumber { get; set; } = string.Empty;

    [JsonPropertyName("oldest_lot_number")]
    public string? OldestLotNumber { get; set; }

    [JsonPropertyName("warning_message")]
    public string? WarningMessage { get; set; }
}

