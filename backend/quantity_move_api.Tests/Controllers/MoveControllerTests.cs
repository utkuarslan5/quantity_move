using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Controllers;

public class MoveControllerTests : ControllerTestBase
{
    private readonly Mock<IQuantityMoveService> _mockMoveService;
    private readonly Mock<IQuantityValidationService> _mockValidationService;
    private readonly Mock<ILogger<MoveController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly MoveController _controller;

    public MoveControllerTests()
    {
        _mockMoveService = new Mock<IQuantityMoveService>();
        _mockValidationService = new Mock<IQuantityValidationService>();
        _mockLogger = CreateMockLogger<MoveController>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _configuration = CreateTestConfiguration();

        _controller = new MoveController(
            _mockMoveService.Object,
            _mockValidationService.Object,
            _mockLogger.Object,
            _configuration,
            _mockConfigurationService.Object);
    }

    #region ValidateMove Tests

    [Fact]
    public async Task ValidateMove_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 10.0m,
            WarehouseCode = "WH001"
        };
        var expectedResponse = new MoveValidationResponse
        {
            IsValid = true,
            SourceValidation = new ValidationResponse { IsValid = true },
            TargetValidation = new ValidationResponse { IsValid = true }
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateMove(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<MoveValidationResponse>;
        response!.Success.Should().BeTrue();
        response.Data!.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateMove_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new MoveValidationRequest();
        _controller.ModelState.AddModelError("ItemCode", "ItemCode is required");

        // Act
        var result = await _controller.ValidateMove(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateMove_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 10.0m
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ValidateMove(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ValidateMove_UsesDefaultWarehouse()
    {
        // Arrange
        var request = new MoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 10.0m,
            WarehouseCode = null
        };
        var expectedResponse = new MoveValidationResponse { IsValid = true };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateMove(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
    }

    #endregion

    #region Move Tests

    [Fact]
    public async Task Move_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var validationResponse = new MoveValidationResponse { IsValid = true };
        var moveResponse = TestHelpers.CreateMoveQuantityResponse(success: true);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(moveResponse);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<MoveQuantityResponse>;
        response!.Success.Should().BeTrue();
        response.Data!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Move_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        _controller.ModelState.AddModelError("ItemCode", "ItemCode is required");

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Move_WithValidationFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var validationResponse = new MoveValidationResponse
        {
            IsValid = false,
            ErrorMessage = "Insufficient stock"
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var response = badRequest!.Value as ApiResponse<MoveQuantityResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task Move_WhenServiceReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var validationResponse = new MoveValidationResponse { IsValid = true };
        var moveResponse = TestHelpers.CreateMoveQuantityResponse(success: false, returnCode: 1);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(moveResponse);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var response = badRequest!.Value as ApiResponse<MoveQuantityResponse>;
        response!.Data!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Move_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var validationResponse = new MoveValidationResponse { IsValid = true };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Move_UsesDefaultWarehouseAndSite()
    {
        // Arrange
        var request = new MoveQuantityRequest
        {
            ItemCode = "ITEM001",
            SourceLocation = "LOC001",
            SourceLotNumber = "LOT001",
            TargetLocation = "LOC002",
            Quantity = 10.0m,
            WarehouseCode = null,
            SiteReference = null
        };
        var validationResponse = new MoveValidationResponse { IsValid = true };
        var moveResponse = TestHelpers.CreateMoveQuantityResponse(success: true);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("DEFAULT_SITE");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(It.IsAny<MoveQuantityRequest>()))
            .ReturnsAsync(moveResponse);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
        _mockConfigurationService.Verify(x => x.GetDefaultSite(), Times.Once);
    }

    [Fact]
    public async Task Move_ValidatesBeforeMoving()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var validationResponse = new MoveValidationResponse { IsValid = true };
        var moveResponse = TestHelpers.CreateMoveQuantityResponse(success: true);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        _mockValidationService.Setup(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()))
            .ReturnsAsync(validationResponse);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(moveResponse);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockValidationService.Verify(x => x.ValidateMoveAsync(It.IsAny<MoveValidationRequest>()), Times.Once);
        _mockMoveService.Verify(x => x.MoveQuantityAsync(request), Times.Once);
    }

    #endregion
}

