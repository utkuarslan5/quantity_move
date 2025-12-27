using quantity_move_api.Models;

namespace quantity_move_api.Services.Quantity;

public interface IQuantityValidationService
{
    // Validate source location has sufficient stock
    Task<ValidationResponse> ValidateSourceStockAsync(string itemCode, string lotNumber, string sourceLocation, decimal quantity, string warehouseCode);

    // Validate target location is valid for receiving
    Task<ValidationResponse> ValidateTargetLocationAsync(string itemCode, string targetLocation, string warehouseCode);

    // Validate move is allowed (combines source + target validation)
    Task<MoveValidationResponse> ValidateMoveAsync(MoveValidationRequest request);
}

