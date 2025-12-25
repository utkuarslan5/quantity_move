using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;
using Dapper;
using System.Data.SqlClient;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocationController> _logger;

    public LocationController(
        IDatabaseService databaseService,
        IConfiguration configuration,
        ILogger<LocationController> logger)
    {
        _databaseService = databaseService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<ValidateLocationResponse>>> Validate([FromBody] ValidateLocationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ValidateLocationResponse>.ErrorResponse("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            // Query location table to validate location code
            // Common table names: loc_mst, location_mst, locations
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, ApiResponse<ValidateLocationResponse>.ErrorResponse("Database connection not configured"));
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
                return NotFound(ApiResponse<ValidateLocationResponse>.SuccessResponse(response, "Location not found"));
            }

            return Ok(ApiResponse<ValidateLocationResponse>.SuccessResponse(response, "Location validated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location validation");
            return StatusCode(500, ApiResponse<ValidateLocationResponse>.ErrorResponse("An error occurred during location validation"));
        }
    }
}

