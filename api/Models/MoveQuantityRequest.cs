using System.ComponentModel.DataAnnotations;

namespace quantity_move_api.Models;

public class MoveQuantityRequest
{
    [Required]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    public string SourceLocation { get; set; } = string.Empty;

    [Required]
    public string TargetLocation { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    public string? Lot { get; set; }
    public string? Warehouse { get; set; }
    public string? Site { get; set; }
    public string? UserId { get; set; }
}

