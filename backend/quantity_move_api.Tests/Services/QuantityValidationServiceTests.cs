using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class QuantityValidationServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<QuantityValidationService>> _mockLogger;
    private readonly QuantityValidationService _service;

    public QuantityValidationServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<QuantityValidationService>>();

        _service = new QuantityValidationService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateSourceStockAsync_WithSufficientStock_ReturnsValid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var sourceLocation = "LOC001";
        var warehouseCode = "WH001";
        var quantity = 50.0m;
        var availableQuantity = 100.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateSourceStockAsync(
            itemCode, lotNumber, sourceLocation, quantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateSourceStockAsync_WithInsufficientStock_ReturnsInvalid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var sourceLocation = "LOC001";
        var warehouseCode = "WH001";
        var quantity = 100.0m;
        var availableQuantity = 50.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateSourceStockAsync(
            itemCode, lotNumber, sourceLocation, quantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("Insufficient stock");
        result.ErrorMessage.Should().Contain(availableQuantity.ToString());
        result.ErrorMessage.Should().Contain(quantity.ToString());
    }

    [Fact]
    public async Task ValidateSourceStockAsync_WithNullResult_ReturnsInvalid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var sourceLocation = "LOC001";
        var warehouseCode = "WH001";
        var quantity = 50.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((decimal?)null);

        // Act
        var result = await _service.ValidateSourceStockAsync(
            itemCode, lotNumber, sourceLocation, quantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateSourceStockAsync_WithExactQuantity_ReturnsValid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var sourceLocation = "LOC001";
        var warehouseCode = "WH001";
        var quantity = 100.0m;
        var availableQuantity = 100.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateSourceStockAsync(
            itemCode, lotNumber, sourceLocation, quantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateSourceStockAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var sourceLocation = "LOC001";
        var warehouseCode = "WH001";
        var quantity = 50.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.ValidateSourceStockAsync(
            itemCode, lotNumber, sourceLocation, quantity, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task ValidateTargetLocationAsync_WithValidLocation_ReturnsValid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var targetLocation = "LOC002";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.ValidateTargetLocationAsync(itemCode, targetLocation, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateTargetLocationAsync_WithInvalidLocation_ReturnsInvalid()
    {
        // Arrange
        var itemCode = "ITEM001";
        var targetLocation = "LOC002";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.ValidateTargetLocationAsync(itemCode, targetLocation, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("Target location");
        result.ErrorMessage.Should().Contain(targetLocation);
        result.ErrorMessage.Should().Contain("not found");
    }

    [Fact]
    public async Task ValidateTargetLocationAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var targetLocation = "LOC002";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.ValidateTargetLocationAsync(itemCode, targetLocation, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task ValidateMoveAsync_WithAllValid_ReturnsValid()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(100.0m);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.SourceValidation.Should().NotBeNull();
        result.SourceValidation!.IsValid.Should().BeTrue();
        result.TargetValidation.Should().NotBeNull();
        result.TargetValidation!.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateMoveAsync_WithInsufficientStock_ReturnsInvalid()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 100.0m,
            WarehouseCode = "WH001"
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(50.0m); // Less than required

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.SourceValidation.Should().NotBeNull();
        result.SourceValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateMoveAsync_WithInvalidTargetLocation_ReturnsInvalid()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(100.0m);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0); // Location not found

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.TargetValidation.Should().NotBeNull();
        result.TargetValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateMoveAsync_WithNullWarehouse_UsesDefault()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = null
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(100.0m);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");

        // Act
        var result = await _service.ValidateMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
    }
}

