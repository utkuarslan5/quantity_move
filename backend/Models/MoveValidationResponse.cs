using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class MoveValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("source_validation")]
    public ValidationResponse? SourceValidation { get; set; }

    [JsonPropertyName("target_validation")]
    public ValidationResponse? TargetValidation { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

