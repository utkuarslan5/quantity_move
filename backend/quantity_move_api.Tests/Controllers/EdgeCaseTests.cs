using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Controllers;

/// <summary>
/// Edge case tests for controllers - null handling, boundary values, invalid inputs.
/// </summary>
public class EdgeCaseTests : ControllerTestBase
{
    #region MoveController Edge Cases

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithNullItemCode_ReturnsBadRequest()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        var request = new MoveQuantityRequest
        {
            ItemCode = null!,
            SourceLocation = "LOC001",
            SourceLotNumber = "LOT001",
            TargetLocation = "LOC002",
            Quantity = 10.0m
        };

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithEmptyItemCode_ReturnsBadRequest()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        var request = new MoveQuantityRequest
        {
            ItemCode = string.Empty,
            SourceLocation = "LOC001",
            SourceLotNumber = "LOT001",
            TargetLocation = "LOC002",
            Quantity = 10.0m
        };

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithZeroQuantity_ReturnsBadRequest()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);

        var request = TestHelpers.CreateMoveQuantityRequest(quantity: 0);

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithNegativeQuantity_ReturnsBadRequest()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);

        var request = TestHelpers.CreateMoveQuantityRequest(quantity: -10.0m);

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithVeryLargeQuantity_ShouldHandleGracefully()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        mockConfigService.Setup(x => x.GetDefaultWarehouse()).Returns("WH001");
        mockConfigService.Setup(x => x.GetDefaultSite()).Returns("SITE001");

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);

        var request = TestHelpers.CreateMoveQuantityRequest(quantity: decimal.MaxValue);

        mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(new MoveValidationResponse { IsValid = false, ErrorMessage = "Quantity too large" });

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_ValidateMove_WithNullRequest_ReturnsBadRequest()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);

        // Act
        var result = await controller.ValidateMove(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region StockController Edge Cases

    [Fact]
    [Trait("Category", "Unit")]
    public async Task StockController_GetStockByBarcode_WithNullBarcode_ReturnsBadRequest()
    {
        // Arrange
        var mockQueryService = new Mock<IStockQueryService>();
        var mockLogger = CreateMockLogger<StockController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new StockController(
            mockQueryService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        // Act
        var result = await controller.GetStockByBarcode(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task StockController_GetStockByBarcode_WithEmptyBarcode_ReturnsBadRequest()
    {
        // Arrange
        var mockQueryService = new Mock<IStockQueryService>();
        var mockLogger = CreateMockLogger<StockController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new StockController(
            mockQueryService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        // Act
        var result = await controller.GetStockByBarcode(string.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task StockController_GetStockByBarcode_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var mockQueryService = new Mock<IStockQueryService>();
        var mockLogger = CreateMockLogger<StockController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new StockController(
            mockQueryService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        // Act - Missing % separator
        var result = await controller.GetStockByBarcode("ITEM001LOT001");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task StockController_GetStockByBarcode_WithMultipleSeparators_ReturnsBadRequest()
    {
        // Arrange
        var mockQueryService = new Mock<IStockQueryService>();
        var mockLogger = CreateMockLogger<StockController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new StockController(
            mockQueryService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        // Act - Too many % separators
        var result = await controller.GetStockByBarcode("ITEM001%LOT001%EXTRA");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task StockController_GetStockByBarcode_WithWhitespaceOnly_ReturnsBadRequest()
    {
        // Arrange
        var mockQueryService = new Mock<IStockQueryService>();
        var mockLogger = CreateMockLogger<StockController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new StockController(
            mockQueryService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        // Act
        var result = await controller.GetStockByBarcode("   ");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region General Edge Cases

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithSameSourceAndTarget_ShouldBeHandled()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        mockConfigService.Setup(x => x.GetDefaultWarehouse()).Returns("WH001");
        mockConfigService.Setup(x => x.GetDefaultSite()).Returns("SITE001");

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        var request = new MoveQuantityRequest
        {
            ItemCode = "ITEM001",
            SourceLocation = "LOC001",
            SourceLotNumber = "LOT001",
            TargetLocation = "LOC001", // Same as source
            Quantity = 10.0m,
            WarehouseCode = "WH001"
        };

        mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(new MoveValidationResponse { IsValid = false, ErrorMessage = "Source and target cannot be the same" });

        // Act
        var result = await controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MoveController_Move_WithVeryLongStrings_ShouldHandleGracefully()
    {
        // Arrange
        var mockMoveService = new Mock<IQuantityMoveService>();
        var mockValidationService = new Mock<IQuantityValidationService>();
        var mockLogger = CreateMockLogger<MoveController>();
        var mockConfigService = new Mock<IConfigurationService>();
        var configuration = CreateTestConfiguration();

        var controller = new MoveController(
            mockMoveService.Object,
            mockValidationService.Object,
            mockLogger.Object,
            configuration,
            mockConfigService.Object);
        
        SetupHttpContext(controller);

        var longString = new string('A', 1000);
        var request = new MoveQuantityRequest
        {
            ItemCode = longString,
            SourceLocation = "LOC001",
            SourceLotNumber = "LOT001",
            TargetLocation = "LOC002",
            Quantity = 10.0m
        };

        // Act
        var result = await controller.Move(request);

        // Assert - Should handle gracefully (either validate or process)
        result.Result.Should().NotBeNull();
    }

    #endregion
}

