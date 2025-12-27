using Dapper;
using quantity_move_api.Models;
using System.Data;

namespace quantity_move_api.Services;

public class QuantityService : IQuantityService
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<QuantityService> _logger;

    public QuantityService(
        IDatabaseService databaseService,
        IConfiguration configuration,
        ILogger<QuantityService> logger)
    {
        _databaseService = databaseService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<MoveQuantityResponse> MoveQuantityAsync(MoveQuantityRequest request)
    {
        var procedureName = _configuration["StoredProcedures:MoveQuantity"] ?? "move_quantity";

        // Build parameters for stored procedure
        var parameters = new DynamicParameters();
        parameters.Add("@item_code", request.ItemCode);
        parameters.Add("@source_location", request.SourceLocation);
        parameters.Add("@source_lot_number", request.SourceLotNumber);
        parameters.Add("@target_location", request.TargetLocation);
        parameters.Add("@quantity", request.Quantity);
        parameters.Add("@warehouse_code", request.WarehouseCode);
        parameters.Add("@site_reference", request.SiteReference ?? (object)DBNull.Value);

        // Add output parameters
        parameters.Add("@transaction_id", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add("@return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@error_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await _databaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters);

        var transactionId = parameters.Get<long?>("@transaction_id");
        var returnCode = parameters.Get<int>("@return_code");
        var errorMessage = parameters.Get<string>("@error_message");

        return new MoveQuantityResponse
        {
            Success = returnCode == 0,
            TransactionId = transactionId,
            ReturnCode = returnCode,
            ErrorMessage = errorMessage
        };
    }
}



