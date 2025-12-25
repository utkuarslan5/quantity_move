using quantity_move_api.Models;

namespace quantity_move_api.Services;

public interface IQuantityService
{
    Task<MoveQuantityResponse> MoveQuantityAsync(MoveQuantityRequest request);
}

