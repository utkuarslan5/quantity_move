using Dapper;
using quantity_move_api.Models;
using System.Data.SqlClient;

namespace quantity_move_api.Services;

public class LocationService : ILocationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IConfiguration configuration, ILogger<LocationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ValidateLocationResponse> ValidateLocationAsync(ValidateLocationRequest request)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection not configured");
        }

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Try multiple possible table/column combinations
        var queries = new[]
        {
            // Option 1: loc_mst table (common in Infor)
            @"SELECT TOP 1 
                loc AS LocationCode,
                loc_desc AS LocationDescription
              FROM loc_mst
              WHERE loc = @LocationCode",
            
            // Option 2: location_mst table
            @"SELECT TOP 1 
                location_code AS LocationCode,
                location_desc AS LocationDescription
              FROM location_mst
              WHERE location_code = @LocationCode",
            
            // Option 3: locations table
            @"SELECT TOP 1 
                code AS LocationCode,
                description AS LocationDescription
              FROM locations
              WHERE code = @LocationCode"
        };

        ValidateLocationResponse? response = null;

        foreach (var query in queries)
        {
            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<ValidateLocationResponse>(
                    query,
                    new { LocationCode = request.LocationCode }
                );

                if (result != null)
                {
                    result.IsValid = true;
                    result.LocationCode = request.LocationCode;
                    response = result;
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Location validation query failed, trying next option");
                // Continue to next query
            }
        }

        if (response == null || !response.IsValid)
        {
            response = new ValidateLocationResponse
            {
                IsValid = false,
                LocationCode = request.LocationCode,
                Message = "Location not found"
            };
        }

        return response;
    }
}

