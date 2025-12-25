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
        var procedureName = _configuration["StoredProcedures:MoveQuantity"] ?? "TR_Miktar_Ilerlet";

        // Build parameters for stored procedure
        // Adjust parameter names based on actual stored procedure signature
        var parameters = new DynamicParameters();
        parameters.Add("@item", request.ItemCode);
        parameters.Add("@sourceLoc", request.SourceLocation);
        parameters.Add("@targetLoc", request.TargetLocation);
        parameters.Add("@qty", request.Quantity);
        
        if (!string.IsNullOrEmpty(request.Lot))
            parameters.Add("@lot", request.Lot);
        if (!string.IsNullOrEmpty(request.Warehouse))
            parameters.Add("@whse", request.Warehouse);
        if (!string.IsNullOrEmpty(request.Site))
            parameters.Add("@site", request.Site);
        if (!string.IsNullOrEmpty(request.UserId))
            parameters.Add("@userId", request.UserId);

        // Add output parameters (adjust based on actual SP signature)
        parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@transactionId", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await _databaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters);

        var returnValue = parameters.Get<int>("@returnValue");
        var rowsAffected = parameters.Get<int?>("@rowsAffected");
        var transactionId = parameters.Get<string>("@transactionId");
        var message = parameters.Get<string>("@message");

        return new MoveQuantityResponse
        {
            Success = returnValue == 0, // Adjust based on actual return code meaning
            ReturnCode = returnValue,
            RowsAffected = rowsAffected,
            TransactionId = transactionId,
            Message = message
        };
    }
}

