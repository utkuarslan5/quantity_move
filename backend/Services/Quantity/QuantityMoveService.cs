using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Fifo;
using System.Data;

namespace quantity_move_api.Services.Quantity;

public class QuantityMoveService : BaseService<QuantityMoveService>, IQuantityMoveService
{
    private readonly IQuantityValidationService _validationService;
    private readonly IFifoService? _fifoService;

    public QuantityMoveService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<QuantityMoveService> logger,
        IQuantityValidationService validationService,
        IFifoService? fifoService = null)
        : base(databaseService, configurationService, logger)
    {
        _validationService = validationService;
        _fifoService = fifoService;
    }

    public async Task<MoveQuantityResponse> MoveQuantityAsync(MoveQuantityRequest request)
    {
        Logger.LogInformation("Moving quantity for item {ItemCode} from {SourceLocation} to {TargetLocation}, quantity {Quantity}", 
            request.ItemCode, request.SourceLocation, request.TargetLocation, request.Quantity);

        var procedureName = ConfigurationService.GetStoredProcedureName("MoveQuantity");

        var parameters = new DynamicParameters();
        parameters.Add("@item_code", request.ItemCode);
        parameters.Add("@source_location", request.SourceLocation);
        parameters.Add("@source_lot_number", request.SourceLotNumber);
        parameters.Add("@target_location", request.TargetLocation);
        parameters.Add("@quantity", request.Quantity);
        parameters.Add("@warehouse_code", request.WarehouseCode);
        parameters.Add("@site_reference", request.SiteReference ?? (object)DBNull.Value);

        parameters.Add("@transaction_id", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add("@return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@error_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await DatabaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters).ConfigureAwait(false);

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

    public async Task<MoveQuantityResponse> MoveQuantityWithValidationAsync(MoveQuantityRequest request)
    {
        // Validate move first
        var validationRequest = new MoveValidationRequest
        {
            ItemCode = request.ItemCode,
            LotNumber = request.SourceLotNumber,
            SourceLocation = request.SourceLocation,
            TargetLocation = request.TargetLocation,
            Quantity = request.Quantity,
            WarehouseCode = request.WarehouseCode
        };

        var validation = await _validationService.ValidateMoveAsync(validationRequest).ConfigureAwait(false);
        if (!validation.IsValid)
        {
            return new MoveQuantityResponse
            {
                Success = false,
                ReturnCode = -1,
                ErrorMessage = validation.ErrorMessage
            };
        }

        // Proceed with move
        return await MoveQuantityAsync(request).ConfigureAwait(false);
    }

    public async Task<MoveQuantityResponse> MoveQuantityWithFifoCheckAsync(MoveQuantityRequest request)
    {
        // Check FIFO compliance if service is available
        if (_fifoService != null)
        {
            var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
            var fifoValidation = await _fifoService.ValidateFifoComplianceAsync(
                request.ItemCode, request.SourceLotNumber, warehouseCode, request.SiteReference).ConfigureAwait(false);

            if (!fifoValidation.IsCompliant && !string.IsNullOrEmpty(fifoValidation.WarningMessage))
            {
                Logger.LogWarning("FIFO violation detected: {WarningMessage}", fifoValidation.WarningMessage);
                // Continue with move but log warning
            }
        }

        // Validate and move
        return await MoveQuantityWithValidationAsync(request).ConfigureAwait(false);
    }
}

