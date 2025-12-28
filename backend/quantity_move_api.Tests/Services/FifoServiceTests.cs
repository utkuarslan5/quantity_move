using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Fifo;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class FifoServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<FifoService>> _mockLogger;
    private readonly FifoService _service;

    public FifoServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<FifoService>>();

        _service = new FifoService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetOldestLotAsync_WithValidData_ReturnsOldestLot()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var oldestLot = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            LocationCode = "LOC001",
            OldestLotNumber = "LOT001",
            QuantityOnHand = 100.0m,
            FifoDate = DateTime.Now.AddDays(-30)
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(oldestLot);

        // Act
        var result = await _service.GetOldestLotAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.OldestLotNumber.Should().Be("LOT001");
        result.LocationCode.Should().Be("LOC001");
        result.QuantityOnHand.Should().Be(100.0m);
    }

    [Fact]
    public async Task GetOldestLotAsync_WithNoLots_ReturnsEmptyResponse()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((FifoOldestLotResponse?)null);

        // Act
        var result = await _service.GetOldestLotAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.OldestLotNumber.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetOldestLotAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var siteReference = "SITE001";
        var oldestLot = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = "LOT001"
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(oldestLot);

        // Act
        var result = await _service.GetOldestLotAsync(itemCode, warehouseCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetOldestLotAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetOldestLotAsync(itemCode, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task ValidateFifoComplianceAsync_WithCompliantLot_ReturnsCompliant()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var oldestLot = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = lotNumber,
            LocationCode = "LOC001",
            FifoDate = DateTime.Now.AddDays(-30)
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(oldestLot);

        // Act
        var result = await _service.ValidateFifoComplianceAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsCompliant.Should().BeTrue();
        result.ItemCode.Should().Be(itemCode);
        result.CurrentLotNumber.Should().Be(lotNumber);
        result.OldestLotNumber.Should().Be(lotNumber);
        result.WarningMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateFifoComplianceAsync_WithNonCompliantLot_ReturnsNonCompliant()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT002";
        var warehouseCode = "WH001";
        var oldestLot = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = "LOT001",
            LocationCode = "LOC001",
            FifoDate = DateTime.Now.AddDays(-30)
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(oldestLot);

        // Act
        var result = await _service.ValidateFifoComplianceAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsCompliant.Should().BeFalse();
        result.ItemCode.Should().Be(itemCode);
        result.CurrentLotNumber.Should().Be(lotNumber);
        result.OldestLotNumber.Should().Be("LOT001");
        result.WarningMessage.Should().NotBeNull();
        result.WarningMessage.Should().Contain("Older lot exists");
        result.WarningMessage.Should().Contain("LOT001");
    }

    [Fact]
    public async Task ValidateFifoComplianceAsync_WithNoLots_ReturnsCompliant()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((FifoOldestLotResponse?)null);

        // Act
        var result = await _service.ValidateFifoComplianceAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.IsCompliant.Should().BeTrue();
        result.ItemCode.Should().Be(itemCode);
        result.CurrentLotNumber.Should().Be(lotNumber);
        result.OldestLotNumber.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateFifoComplianceAsync_WithSiteReference_PassesToGetOldestLot()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var siteReference = "SITE001";
        var oldestLot = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = lotNumber
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(oldestLot);

        // Act
        var result = await _service.ValidateFifoComplianceAsync(itemCode, lotNumber, warehouseCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        result.IsCompliant.Should().BeTrue();
    }

    [Fact]
    public async Task GetFifoSummaryAsync_WithValidData_ReturnsSummary()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var lots = new List<FifoSummaryLot>
        {
            new FifoSummaryLot
            {
                LotNumber = "LOT001",
                LocationCode = "LOC001",
                QuantityOnHand = 100.0m,
                FifoDate = DateTime.Now.AddDays(-30)
            },
            new FifoSummaryLot
            {
                LotNumber = "LOT002",
                LocationCode = "LOC002",
                QuantityOnHand = 50.0m,
                FifoDate = DateTime.Now.AddDays(-20)
            }
        };

        _mockQueryService.Setup(x => x.QueryAsync<FifoSummaryLot>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(lots);

        // Act
        var result = await _service.GetFifoSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.WarehouseCode.Should().Be(warehouseCode);
        result.Lots.Should().HaveCount(2);
        result.Lots.Should().Contain(l => l.LotNumber == "LOT001");
        result.Lots.Should().Contain(l => l.LotNumber == "LOT002");
    }

    [Fact]
    public async Task GetFifoSummaryAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var emptyLots = new List<FifoSummaryLot>();

        _mockQueryService.Setup(x => x.QueryAsync<FifoSummaryLot>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(emptyLots);

        // Act
        var result = await _service.GetFifoSummaryAsync(itemCode, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.ItemCode.Should().Be(itemCode);
        result.WarehouseCode.Should().Be(warehouseCode);
        result.Lots.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFifoSummaryAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var siteReference = "SITE001";
        var lots = new List<FifoSummaryLot>();

        _mockQueryService.Setup(x => x.QueryAsync<FifoSummaryLot>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(lots);

        // Act
        var result = await _service.GetFifoSummaryAsync(itemCode, warehouseCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryAsync<FifoSummaryLot>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetFifoSummaryAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryAsync<FifoSummaryLot>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetFifoSummaryAsync(itemCode, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}

