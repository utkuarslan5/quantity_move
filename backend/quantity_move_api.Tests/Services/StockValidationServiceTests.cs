using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class StockValidationServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<StockValidationService>> _mockLogger;
    private readonly StockValidationService _service;

    public StockValidationServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<StockValidationService>>();

        _service = new StockValidationService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateItemAsync_WithValidLotTrackedItem_ReturnsValidResponse()
    {
        // Arrange
        var itemCode = "ITEM001";
        var itemResponse = new ItemValidationResponse
        {
            ItemCode = itemCode,
            IsLotTracked = true,
            Description = "Test Item",
            IsValid = false // Will be set to true by service
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        // Act
        var result = await _service.ValidateItemAsync(itemCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.IsValid.Should().BeTrue();
        result.IsLotTracked.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateItemAsync_WithNonLotTrackedItem_ReturnsValidWithErrorMessage()
    {
        // Arrange
        var itemCode = "ITEM001";
        var itemResponse = new ItemValidationResponse
        {
            ItemCode = itemCode,
            IsLotTracked = false,
            Description = "Test Item",
            IsValid = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        // Act
        var result = await _service.ValidateItemAsync(itemCode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().Be("Item is not lot-tracked");
    }

    [Fact]
    public async Task ValidateItemAsync_WithItemNotFound_ReturnsInvalidResponse()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((ItemValidationResponse?)null);

        // Act
        var result = await _service.ValidateItemAsync(itemCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Item not found");
    }

    [Fact]
    public async Task ValidateItemAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var siteReference = "SITE001";
        var itemResponse = new ItemValidationResponse
        {
            ItemCode = itemCode,
            IsLotTracked = true,
            IsValid = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        // Act
        var result = await _service.ValidateItemAsync(itemCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task ValidateLotAsync_WithValidLot_ReturnsValidResponse()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.ValidateLotAsync(itemCode, lotNumber);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.LotNumber.Should().Be(lotNumber);
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateLotAsync_WithLotNotFound_ReturnsInvalidResponse()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.ValidateLotAsync(itemCode, lotNumber);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.LotNumber.Should().Be(lotNumber);
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Lot not found");
    }

    [Fact]
    public async Task ValidateLocationAsync_WithValidLocation_ReturnsValidResponse()
    {
        // Arrange
        var locationCode = "LOC001";
        var locationResponse = new LocationValidationResponse
        {
            LocationCode = locationCode,
            LocationType = "Warehouse",
            Description = "Test Location",
            IsValid = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse);

        // Act
        var result = await _service.ValidateLocationAsync(locationCode);

        // Assert
        result.Should().NotBeNull();
        result.LocationCode.Should().Be(locationCode);
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateLocationAsync_WithLocationNotFound_ReturnsInvalidResponse()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((LocationValidationResponse?)null);

        // Act
        var result = await _service.ValidateLocationAsync(locationCode);

        // Assert
        result.Should().NotBeNull();
        result.LocationCode.Should().Be(locationCode);
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Location not found");
    }

    [Fact]
    public async Task ValidateLocationAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var locationCode = "LOC001";
        var siteReference = "SITE001";
        var locationResponse = new LocationValidationResponse
        {
            LocationCode = locationCode,
            IsValid = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse);

        // Act
        var result = await _service.ValidateLocationAsync(locationCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task ValidateStockAvailabilityAsync_WithSufficientStock_ReturnsAvailable()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";
        var requiredQuantity = 50.0m;
        var availableQuantity = 100.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateStockAvailabilityAsync(
            itemCode, lotNumber, locationCode, requiredQuantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsAvailable.Should().BeTrue();
        result.ItemCode.Should().Be(itemCode);
        result.LotNumber.Should().Be(lotNumber);
        result.LocationCode.Should().Be(locationCode);
        result.AvailableQuantity.Should().Be(availableQuantity);
        result.RequiredQuantity.Should().Be(requiredQuantity);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateStockAvailabilityAsync_WithInsufficientStock_ReturnsNotAvailable()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";
        var requiredQuantity = 100.0m;
        var availableQuantity = 50.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateStockAvailabilityAsync(
            itemCode, lotNumber, locationCode, requiredQuantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsAvailable.Should().BeFalse();
        result.AvailableQuantity.Should().Be(availableQuantity);
        result.RequiredQuantity.Should().Be(requiredQuantity);
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task ValidateStockAvailabilityAsync_WithNullResult_ReturnsNotAvailable()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";
        var requiredQuantity = 50.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((decimal?)null);

        // Act
        var result = await _service.ValidateStockAvailabilityAsync(
            itemCode, lotNumber, locationCode, requiredQuantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsAvailable.Should().BeFalse();
        result.AvailableQuantity.Should().Be(0);
    }

    [Fact]
    public async Task ValidateStockAvailabilityAsync_WithExactQuantity_ReturnsAvailable()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";
        var requiredQuantity = 100.0m;
        var availableQuantity = 100.0m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(availableQuantity);

        // Act
        var result = await _service.ValidateStockAvailabilityAsync(
            itemCode, lotNumber, locationCode, requiredQuantity, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithAllValid_ReturnsValidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001",
            SiteReference = "SITE001"
        };

        var itemResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = false
        };

        var locationResponse = new LocationValidationResponse
        {
            LocationCode = request.SourceLocation,
            IsValid = false
        };

        var targetLocationResponse = new LocationValidationResponse
        {
            LocationCode = request.TargetLocation,
            IsValid = false
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse)
            .ReturnsAsync(targetLocationResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(100.0m);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ItemValidation.Should().NotBeNull();
        result.LotValidation.Should().NotBeNull();
        result.SourceLocationValidation.Should().NotBeNull();
        result.TargetLocationValidation.Should().NotBeNull();
        result.StockAvailability.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithInvalidItem_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((ItemValidationResponse?)null);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ItemValidation.Should().NotBeNull();
        result.ItemValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithInvalidLot_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        var itemResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.LotValidation.Should().NotBeNull();
        result.LotValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithInvalidSourceLocation_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        var itemResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = false
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((LocationValidationResponse?)null);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.SourceLocationValidation.Should().NotBeNull();
        result.SourceLocationValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Source location invalid");
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithInvalidTargetLocation_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001"
        };

        var itemResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = false
        };

        var sourceLocationResponse = new LocationValidationResponse
        {
            LocationCode = request.SourceLocation,
            IsValid = false
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(sourceLocationResponse)
            .ReturnsAsync((LocationValidationResponse?)null);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.TargetLocationValidation.Should().NotBeNull();
        result.TargetLocationValidation!.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Target location invalid");
    }

    [Fact]
    public async Task ValidateStockForMoveAsync_WithInsufficientStock_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 100.0m,
            WarehouseCode = "WH001"
        };

        var itemResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = false
        };

        var locationResponse = new LocationValidationResponse
        {
            LocationCode = request.SourceLocation,
            IsValid = false
        };

        var targetLocationResponse = new LocationValidationResponse
        {
            LocationCode = request.TargetLocation,
            IsValid = false
        };

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse)
            .ReturnsAsync(targetLocationResponse);

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(50.0m); // Less than required

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        // Act
        var result = await _service.ValidateStockForMoveAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.StockAvailability.Should().NotBeNull();
        result.StockAvailability!.IsAvailable.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }
}

