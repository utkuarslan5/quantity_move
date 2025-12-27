using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class MoveQuantityResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("transaction_id")]
    public long? TransactionId { get; set; }

    [JsonPropertyName("return_code")]
    public int ReturnCode { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

