using quantity_move_api.Models;

namespace quantity_move_api.Services;

public interface ILocationService
{
    Task<ValidateLocationResponse> ValidateLocationAsync(ValidateLocationRequest request);
}

