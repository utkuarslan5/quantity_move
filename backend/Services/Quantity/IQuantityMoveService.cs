using quantity_move_api.Models;

namespace quantity_move_api.Services.Quantity;

public interface IQuantityMoveService
{
    // Simple move (standard scenario)
    Task<MoveQuantityResponse> MoveQuantityAsync(MoveQuantityRequest request);

    // Move with validation (composed operation)
    Task<MoveQuantityResponse> MoveQuantityWithValidationAsync(MoveQuantityRequest request);

    // Move with FIFO check
    Task<MoveQuantityResponse> MoveQuantityWithFifoCheckAsync(MoveQuantityRequest request);
}

