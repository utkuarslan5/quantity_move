using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class ValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

