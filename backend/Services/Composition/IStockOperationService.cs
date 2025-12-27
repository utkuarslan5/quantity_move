using quantity_move_api.Models;

namespace quantity_move_api.Services.Composition;

public interface IStockOperationService
{
    // Composes multiple smaller operations for complex workflows
    Task<MoveQuantityResponse> MoveQuantityWithFullValidationAsync(MoveQuantityRequest request, MoveQuantityOptions options);
}

