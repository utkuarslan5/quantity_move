using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class StockQueryServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<StockQueryService>> _mockLogger;
    private readonly StockQueryService _service;

    public StockQueryServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<StockQueryService>>();

        _service = new StockQueryService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetQuantityAtLocationAsync_WithValidData_ReturnsQuantity()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";
        var expectedQuantity = 100.5m;

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(expectedQuantity);

        // Act
        var result = await _service.GetQuantityAtLocationAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().Be(expectedQuantity);
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<decimal?>(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetQuantityAtLocationAsync_WithNullResult_ReturnsZero()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((decimal?)null);

        // Act
        var result = await _service.GetQuantityAtLocationAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetQuantityAtLocationAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var locationCode = "LOC001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<decimal?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetQuantityAtLocationAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task GetLocationsForItemLotAsync_WithIncludeZeroQuantity_ReturnsAllLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationItem>
        {
            new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m, LocationDisplay = "LOC001-100.00" },
            new LocationItem { LocationCode = "LOC002", QuantityOnHand = 0.0m, LocationDisplay = "LOC002-0.00" },
            new LocationItem { LocationCode = "LOC003", QuantityOnHand = 50.0m, LocationDisplay = "LOC003-50.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetLocationsForItemLotAsync(itemCode, lotNumber, warehouseCode, includeZeroQuantity: true);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(3);
        result.Locations.Should().Contain(l => l.LocationCode == "LOC001");
        result.Locations.Should().Contain(l => l.LocationCode == "LOC002");
        result.Locations.Should().Contain(l => l.LocationCode == "LOC003");
    }

    [Fact]
    public async Task GetLocationsForItemLotAsync_WithoutIncludeZeroQuantity_ReturnsOnlyNonZeroLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationItem>
        {
            new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m, LocationDisplay = "LOC001-100.00" },
            new LocationItem { LocationCode = "LOC003", QuantityOnHand = 50.0m, LocationDisplay = "LOC003-50.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetLocationsForItemLotAsync(itemCode, lotNumber, warehouseCode, includeZeroQuantity: false);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(2);
        result.Locations.Should().NotContain(l => l.QuantityOnHand == 0);
    }

    [Fact]
    public async Task GetLocationsForItemLotAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetLocationsForItemLotAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WithValidData_ReturnsLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<CurrentLocationItem>
        {
            new CurrentLocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m },
            new CurrentLocationItem { LocationCode = "LOC002", QuantityOnHand = 50.0m }
        };

        _mockQueryService.Setup(x => x.QueryAsync<CurrentLocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetCurrentLocationAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.LotNumber.Should().Be(lotNumber);
        result.Locations.Should().HaveCount(2);
        result.Locations.Should().Contain(l => l.LocationCode == "LOC001");
        result.Locations.Should().Contain(l => l.LocationCode == "LOC002");
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var emptyLocations = new List<CurrentLocationItem>();

        _mockQueryService.Setup(x => x.QueryAsync<CurrentLocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(emptyLocations);

        // Act
        var result = await _service.GetCurrentLocationAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryAsync<CurrentLocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetCurrentLocationAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task GetStockSummaryAsync_WithValidData_ReturnsSummary()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var locations = new List<StockSummaryLocation>
        {
            new StockSummaryLocation { LocationCode = "LOC001", LotNumber = "LOT001", QuantityOnHand = 100.0m },
            new StockSummaryLocation { LocationCode = "LOC001", LotNumber = "LOT002", QuantityOnHand = 50.0m },
            new StockSummaryLocation { LocationCode = "LOC002", LotNumber = "LOT001", QuantityOnHand = 25.0m }
        };

        _mockQueryService.Setup(x => x.QueryAsync<StockSummaryLocation>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetStockSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.WarehouseCode.Should().Be(warehouseCode);
        result.LocationCount.Should().Be(3);
        result.TotalQuantity.Should().Be(175.0m);
        result.Locations.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetStockSummaryAsync_WithEmptyResult_ReturnsZeroSummary()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var emptyLocations = new List<StockSummaryLocation>();

        _mockQueryService.Setup(x => x.QueryAsync<StockSummaryLocation>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(emptyLocations);

        // Act
        var result = await _service.GetStockSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.LocationCount.Should().Be(0);
        result.TotalQuantity.Should().Be(0);
        result.Locations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStockSummaryAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryAsync<StockSummaryLocation>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetStockSummaryAsync(itemCode, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}

