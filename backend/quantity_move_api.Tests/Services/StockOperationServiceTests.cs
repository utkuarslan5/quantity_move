using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Composition;
using quantity_move_api.Services.Fifo;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class StockOperationServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IStockValidationService> _mockValidationService;
    private readonly Mock<IQuantityMoveService> _mockMoveService;
    private readonly Mock<IFifoService> _mockFifoService;
    private readonly Mock<ILogger<StockOperationService>> _mockLogger;
    private readonly StockOperationService _service;

    public StockOperationServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockValidationService = new Mock<IStockValidationService>();
        _mockMoveService = new Mock<IQuantityMoveService>();
        _mockFifoService = new Mock<IFifoService>();
        _mockLogger = new Mock<ILogger<StockOperationService>>();

        _service = new StockOperationService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockValidationService.Object,
            _mockMoveService.Object,
            _mockFifoService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithAllValid_ReturnsSuccess()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = false };
        var defaultWarehouse = "WH001";

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode,
                request.SourceLotNumber,
                request.SourceLocation,
                request.Quantity,
                defaultWarehouse))
            .ReturnsAsync(new StockAvailabilityResponse
            {
                IsAvailable = true,
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                LocationCode = request.SourceLocation
            });

        _mockValidationService.Setup(x => x.ValidateLocationAsync(request.TargetLocation, request.SiteReference))
            .ReturnsAsync(new LocationValidationResponse
            {
                LocationCode = request.TargetLocation,
                IsValid = true
            });

        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(new MoveQuantityResponse
            {
                Success = true,
                TransactionId = 12345L,
                ReturnCode = 0
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockValidationService.Verify(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference), Times.Once);
        _mockValidationService.Verify(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber), Times.Once);
        _mockMoveService.Verify(x => x.MoveQuantityAsync(request), Times.Once);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithInvalidItem_ReturnsFailure()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = false };

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsValid = false,
                ErrorMessage = "Item not found"
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ReturnCode.Should().Be(-1);
        result.ErrorMessage.Should().Contain("Item validation failed");
        _mockMoveService.Verify(x => x.MoveQuantityAsync(It.IsAny<MoveQuantityRequest>()), Times.Never);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithInvalidLot_ReturnsFailure()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = false };

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = false,
                ErrorMessage = "Lot not found"
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ReturnCode.Should().Be(-1);
        result.ErrorMessage.Should().Contain("Lot validation failed");
        _mockMoveService.Verify(x => x.MoveQuantityAsync(It.IsAny<MoveQuantityRequest>()), Times.Never);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithInsufficientStock_ReturnsFailure()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = false };
        var defaultWarehouse = "WH001";

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode,
                request.SourceLotNumber,
                request.SourceLocation,
                request.Quantity,
                defaultWarehouse))
            .ReturnsAsync(new StockAvailabilityResponse
            {
                IsAvailable = false,
                ErrorMessage = "Insufficient stock"
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Insufficient stock");
        _mockMoveService.Verify(x => x.MoveQuantityAsync(It.IsAny<MoveQuantityRequest>()), Times.Never);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithInvalidTargetLocation_ReturnsFailure()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = false };
        var defaultWarehouse = "WH001";

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode,
                request.SourceLotNumber,
                request.SourceLocation,
                request.Quantity,
                defaultWarehouse))
            .ReturnsAsync(new StockAvailabilityResponse
            {
                IsAvailable = true
            });

        _mockValidationService.Setup(x => x.ValidateLocationAsync(request.TargetLocation, request.SiteReference))
            .ReturnsAsync(new LocationValidationResponse
            {
                LocationCode = request.TargetLocation,
                IsValid = false,
                ErrorMessage = "Location not found"
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Target location validation failed");
        _mockMoveService.Verify(x => x.MoveQuantityAsync(It.IsAny<MoveQuantityRequest>()), Times.Never);
    }

    [Fact]
    public async Task MoveQuantityWithFullValidationAsync_WithFifoValidation_ValidatesFifo()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = true };
        var defaultWarehouse = "WH001";

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode,
                request.SourceLotNumber,
                request.SourceLocation,
                request.Quantity,
                defaultWarehouse))
            .ReturnsAsync(new StockAvailabilityResponse
            {
                IsAvailable = true
            });

        _mockValidationService.Setup(x => x.ValidateLocationAsync(request.TargetLocation, request.SiteReference))
            .ReturnsAsync(new LocationValidationResponse
            {
                LocationCode = request.TargetLocation,
                IsValid = true
            });

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

        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(new MoveQuantityResponse
            {
                Success = true,
                TransactionId = 12345L,
                ReturnCode = 0
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

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
    public async Task MoveQuantityWithFullValidationAsync_WithFifoViolation_LogsWarningAndContinues()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var options = new MoveQuantityOptions { ValidateFifo = true };
        var defaultWarehouse = "WH001";

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(new ItemValidationResponse
            {
                ItemCode = request.ItemCode,
                IsLotTracked = true,
                IsValid = true
            });

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.SourceLotNumber))
            .ReturnsAsync(new LotValidationResponse
            {
                ItemCode = request.ItemCode,
                LotNumber = request.SourceLotNumber,
                IsValid = true
            });

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode,
                request.SourceLotNumber,
                request.SourceLocation,
                request.Quantity,
                defaultWarehouse))
            .ReturnsAsync(new StockAvailabilityResponse
            {
                IsAvailable = true
            });

        _mockValidationService.Setup(x => x.ValidateLocationAsync(request.TargetLocation, request.SiteReference))
            .ReturnsAsync(new LocationValidationResponse
            {
                LocationCode = request.TargetLocation,
                IsValid = true
            });

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
                WarningMessage = "Older lot exists"
            });

        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(new MoveQuantityResponse
            {
                Success = true,
                TransactionId = 12345L,
                ReturnCode = 0
            });

        // Act
        var result = await _service.MoveQuantityWithFullValidationAsync(request, options);

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

