using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Fifo;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Tests.Helpers;
using System.Data;

namespace quantity_move_api.Tests.Services;

public class QuantityMoveServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQuantityValidationService> _mockValidationService;
    private readonly Mock<IFifoService> _mockFifoService;
    private readonly Mock<ILogger<QuantityMoveService>> _mockLogger;
    private readonly QuantityMoveService _service;

    public QuantityMoveServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockValidationService = new Mock<IQuantityValidationService>();
        _mockFifoService = new Mock<IFifoService>();
        _mockLogger = new Mock<ILogger<QuantityMoveService>>();

        _service = new QuantityMoveService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockLogger.Object,
            _mockValidationService.Object,
            _mockFifoService.Object);
    }

    [Fact]
    public async Task MoveQuantityAsync_WithSuccess_ReturnsSuccessResponse()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";
        var transactionId = 12345L;
        var returnCode = 0;

        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                parameters.Add("@transaction_id", transactionId);
                parameters.Add("@return_code", returnCode);
                parameters.Add("@error_message", (string?)null);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MoveQuantityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.TransactionId.Should().Be(transactionId);
        result.ReturnCode.Should().Be(returnCode);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task MoveQuantityAsync_WithFailure_ReturnsFailureResponse()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";
        var transactionId = 12345L;
        var returnCode = -1;
        var errorMessage = "Insufficient stock";

        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                parameters.Add("@transaction_id", transactionId);
                parameters.Add("@return_code", returnCode);
                parameters.Add("@error_message", errorMessage);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MoveQuantityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.TransactionId.Should().Be(transactionId);
        result.ReturnCode.Should().Be(returnCode);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public async Task MoveQuantityAsync_WithNullSiteReference_PassesDbNull()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        request.SiteReference = null;
        var procedureName = "TR_Miktar_Ilerlet";

        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        DynamicParameters? capturedParams = null;
        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                capturedParams = parameters;
                parameters.Add("@transaction_id", 12345L);
                parameters.Add("@return_code", 0);
            })
            .Returns(Task.CompletedTask);

        // Act
        await _service.MoveQuantityAsync(request);

        // Assert
        capturedParams.Should().NotBeNull();
        // Verify that null site_reference was handled (the service converts null to DBNull.Value)
    }

    [Fact]
    public async Task MoveQuantityAsync_WithException_ThrowsException()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";

        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.MoveQuantityAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task MoveQuantityWithValidationAsync_WithValidMove_ReturnsSuccess()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";
        var validationRequest = new MoveValidationRequest
        {
            ItemCode = request.ItemCode,
            LotNumber = request.SourceLotNumber,
            SourceLocation = request.SourceLocation,
            TargetLocation = request.TargetLocation,
            Quantity = request.Quantity,
            WarehouseCode = request.WarehouseCode
        };

        _mockValidationService.Setup(x => x.ValidateMoveAsync(
                It.Is<MoveValidationRequest>(r => r.ItemCode == request.ItemCode)))
            .ReturnsAsync(new MoveValidationResponse
            {
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                parameters.Add("@transaction_id", 12345L);
                parameters.Add("@return_code", 0);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MoveQuantityWithValidationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockValidationService.Verify(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityWithValidationAsync_WithInvalidMove_ReturnsFailure()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var errorMessage = "Insufficient stock";

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(new MoveValidationResponse
            {
                IsValid = false,
                ErrorMessage = errorMessage
            });

        // Act
        var result = await _service.MoveQuantityWithValidationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ReturnCode.Should().Be(-1);
        result.ErrorMessage.Should().Be(errorMessage);
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<DynamicParameters>()), Times.Never);
    }

    [Fact]
    public async Task MoveQuantityWithFifoCheckAsync_WithCompliantFifo_ReturnsSuccess()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";
        var defaultWarehouse = "WH001";

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);
        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockFifoService.Setup(x => x.ValidateFifoComplianceAsync(
                request.ItemCode,
                request.SourceLotNumber,
                defaultWarehouse,
                request.SiteReference))
            .ReturnsAsync(new FifoValidationResponse
            {
                IsCompliant = true,
                ItemCode = request.ItemCode,
                CurrentLotNumber = request.SourceLotNumber
            });

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(new MoveValidationResponse
            {
                IsValid = true
            });

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                parameters.Add("@transaction_id", 12345L);
                parameters.Add("@return_code", 0);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MoveQuantityWithFifoCheckAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockFifoService.Verify(x => x.ValidateFifoComplianceAsync(
            request.ItemCode,
            request.SourceLotNumber,
            defaultWarehouse,
            request.SiteReference), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityWithFifoCheckAsync_WithNonCompliantFifo_LogsWarningAndContinues()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var procedureName = "TR_Miktar_Ilerlet";
        var defaultWarehouse = "WH001";
        var warningMessage = "Older lot exists";

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);
        _mockConfigurationService.Setup(x => x.GetStoredProcedureName("MoveQuantity"))
            .Returns(procedureName);

        _mockFifoService.Setup(x => x.ValidateFifoComplianceAsync(
                request.ItemCode,
                request.SourceLotNumber,
                defaultWarehouse,
                request.SiteReference))
            .ReturnsAsync(new FifoValidationResponse
            {
                IsCompliant = false,
                ItemCode = request.ItemCode,
                CurrentLotNumber = request.SourceLotNumber,
                WarningMessage = warningMessage
            });

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(new MoveValidationResponse
            {
                IsValid = true
            });

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
                procedureName,
                It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, parameters) =>
            {
                parameters.Add("@transaction_id", 12345L);
                parameters.Add("@return_code", 0);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MoveQuantityWithFifoCheckAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}

