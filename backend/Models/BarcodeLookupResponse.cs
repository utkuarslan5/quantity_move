using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class BarcodeLookupResponse
{
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; set; }

    [JsonPropertyName("lot_number")]
    public string? LotNumber { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("found")]
    public bool Found { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

