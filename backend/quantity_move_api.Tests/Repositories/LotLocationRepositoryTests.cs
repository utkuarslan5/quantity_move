using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Repositories;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Repositories;

public class LotLocationRepositoryTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<LotLocationRepository>> _mockLogger;
    private readonly LotLocationRepository _repository;

    public LotLocationRepositoryTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<LotLocationRepository>>();

        _repository = new LotLocationRepository(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetQuantityAsync_WithValidData_ReturnsQuantity()
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
        var result = await _repository.GetQuantityAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().Be(expectedQuantity);
    }

    [Fact]
    public async Task GetQuantityAsync_WithNullResult_ReturnsZero()
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
        var result = await _repository.GetQuantityAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetLocationsAsync_WithIncludeZeroQuantity_ReturnsAllLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationItem>
        {
            new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m, LocationDisplay = "LOC001-100.00" },
            new LocationItem { LocationCode = "LOC002", QuantityOnHand = 0.0m, LocationDisplay = "LOC002-0.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _repository.GetLocationsAsync(itemCode, lotNumber, warehouseCode, includeZeroQuantity: true);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(l => l.LocationCode == "LOC001");
        result.Should().Contain(l => l.LocationCode == "LOC002");
    }

    [Fact]
    public async Task GetLocationsAsync_WithoutIncludeZeroQuantity_ReturnsOnlyNonZeroLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationItem>
        {
            new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m, LocationDisplay = "LOC001-100.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _repository.GetLocationsAsync(itemCode, lotNumber, warehouseCode, includeZeroQuantity: false);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(l => l.LocationCode == "LOC001");
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
            new StockSummaryLocation { LocationCode = "LOC002", LotNumber = "LOT002", QuantityOnHand = 50.0m }
        };

        _mockQueryService.Setup(x => x.QueryAsync<StockSummaryLocation>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _repository.GetStockSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(l => l.LocationCode == "LOC001" && l.LotNumber == "LOT001");
        result.Should().Contain(l => l.LocationCode == "LOC002" && l.LotNumber == "LOT002");
    }

    [Fact]
    public async Task GetStockSummaryAsync_WithEmptyResult_ReturnsEmptyList()
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
        var result = await _repository.GetStockSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableQuantityAsync_WithValidData_ReturnsQuantity()
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
        var result = await _repository.GetAvailableQuantityAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().Be(expectedQuantity);
    }

    [Fact]
    public async Task GetAvailableQuantityAsync_WithNullResult_ReturnsNull()
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
        var result = await _repository.GetAvailableQuantityAsync(itemCode, lotNumber, locationCode, warehouseCode);

        // Assert
        result.Should().BeNull();
    }
}

