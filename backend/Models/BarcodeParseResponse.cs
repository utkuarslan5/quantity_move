using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class BarcodeParseResponse
{
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; set; }

    [JsonPropertyName("lot_number")]
    public string? LotNumber { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

