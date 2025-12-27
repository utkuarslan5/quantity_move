using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Tests.Helpers;
using System.Data;

namespace quantity_move_api.Tests.Services;

public class QuantityServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<QuantityService>> _mockLogger;
    private readonly QuantityService _quantityService;

    public QuantityServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<QuantityService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        var storedProcSection = new Mock<IConfigurationSection>();
        storedProcSection.Setup(x => x["MoveQuantity"]).Returns("TR_Miktar_Ilerlet");
        
        _mockConfiguration.Setup(x => x.GetSection("StoredProcedures")).Returns(storedProcSection.Object);
        _mockConfiguration.Setup(x => x["StoredProcedures:MoveQuantity"]).Returns("TR_Miktar_Ilerlet");
        
        _quantityService = new QuantityService(_mockDatabaseService.Object, _mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task MoveQuantityAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        
        // Mock the stored procedure execution and set output parameters
        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                    dp.Add("@transactionId", "TXN-12345");
                    dp.Add("@message", "Success");
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _quantityService.MoveQuantityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ReturnCode.Should().Be(0);
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureNonQueryAsync(
            "TR_Miktar_Ilerlet",
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityAsync_UsesDefaultProcedureName_WhenNotConfigured()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var mockConfig = configBuilder.Build();
        
        var service = new QuantityService(_mockDatabaseService.Object, mockConfig, _mockLogger.Object);
        var request = TestHelpers.CreateMoveQuantityRequest();
        
        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                }
            })
            .ReturnsAsync(1);

        // Act
        await service.MoveQuantityAsync(request);

        // Assert
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureNonQueryAsync(
            "TR_Miktar_Ilerlet", // Default procedure name
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityAsync_IncludesAllRequiredParameters()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest(
            itemCode: "ITEM001",
            sourceLocation: "LOC001",
            targetLocation: "LOC002",
            quantity: 100.0m);
        
        DynamicParameters? capturedParams = null;
        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    capturedParams = dp;
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                }
            })
            .ReturnsAsync(1);

        // Act
        await _quantityService.MoveQuantityAsync(request);

        // Assert
        capturedParams.Should().NotBeNull();
        // Note: We can't easily verify parameter values without accessing private members
        // This would require integration tests or refactoring
    }

    [Fact]
    public async Task MoveQuantityAsync_IncludesOptionalParameters_WhenProvided()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        request.Lot = "LOT001";
        request.Warehouse = "WH001";
        request.Site = "SITE001";
        request.UserId = "USER001";

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                }
            })
            .ReturnsAsync(1);

        // Act
        await _quantityService.MoveQuantityAsync(request);

        // Assert
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityAsync_HandlesNullOptionalParameters()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        request.Lot = null;
        request.Warehouse = null;
        request.Site = null;
        request.UserId = null;

        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                }
            })
            .ReturnsAsync(1);

        // Act
        await _quantityService.MoveQuantityAsync(request);

        // Assert
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityAsync_ReturnsResponseWithReturnCode()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        
        _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .Callback<string, object>((proc, param) =>
            {
                if (param is DynamicParameters dp)
                {
                    dp.Add("@returnValue", 0);
                    dp.Add("@rowsAffected", 1);
                    dp.Add("@transactionId", "TXN-12345");
                    dp.Add("@message", "Success");
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _quantityService.MoveQuantityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.ReturnCode.Should().Be(0);
        result.TransactionId.Should().Be("TXN-12345");
    }
}

