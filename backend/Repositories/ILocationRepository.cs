using quantity_move_api.Models;

namespace quantity_move_api.Repositories;

public interface ILocationRepository
{
    Task<LocationValidationResponse?> GetLocationAsync(string locationCode, string? siteReference = null);
    Task<LocationDetailsResponse?> GetLocationDetailsAsync(string locationCode, string? siteReference = null);
    Task<bool> LocationExistsAsync(string locationCode, string? siteReference = null);
}

