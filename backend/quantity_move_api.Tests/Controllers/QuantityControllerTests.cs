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

public class QuantityControllerTests
{
    private readonly Mock<IQuantityMoveService> _mockMoveService;
    private readonly Mock<IQuantityValidationService> _mockValidationService;
    private readonly Mock<ILogger<QuantityController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly QuantityController _controller;

    public QuantityControllerTests()
    {
        _mockMoveService = new Mock<IQuantityMoveService>();
        _mockValidationService = new Mock<IQuantityValidationService>();
        _mockLogger = new Mock<ILogger<QuantityController>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _controller = new QuantityController(_mockMoveService.Object, _mockValidationService.Object, _mockLogger.Object, _mockConfiguration.Object, _mockConfigurationService.Object);
    }

    [Fact]
    public async Task Move_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var response = TestHelpers.CreateMoveQuantityResponse(success: true);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<ApiResponse<MoveQuantityResponse>>();
        
        var apiResponse = okResult.Value as ApiResponse<MoveQuantityResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Success.Should().BeTrue();
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
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ApiResponse<MoveQuantityResponse>>();
        
        var response = badRequestResult.Value as ApiResponse<MoveQuantityResponse>;
        response!.Success.Should().BeFalse();
        response.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Move_WhenServiceReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var response = TestHelpers.CreateMoveQuantityResponse(success: false, returnCode: 1);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ApiResponse<MoveQuantityResponse>>();
        
        var apiResponse = badRequestResult.Value as ApiResponse<MoveQuantityResponse>;
        apiResponse!.Success.Should().BeTrue(); // API response wrapper is success
        apiResponse.Data!.Success.Should().BeFalse(); // But operation failed
    }

    [Fact]
    public async Task Move_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<ApiResponse<MoveQuantityResponse>>();
        
        var response = objectResult.Value as ApiResponse<MoveQuantityResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("error occurred");
    }

    [Fact]
    public async Task Move_WithValidRequest_ReturnsTransactionId()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest();
        var response = TestHelpers.CreateMoveQuantityResponse(
            success: true,
            transactionId: 12345);
        _mockMoveService.Setup(x => x.MoveQuantityAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<MoveQuantityResponse>;
        
        apiResponse!.Data!.TransactionId.Should().Be(12345);
        apiResponse.Data.ReturnCode.Should().Be(0);
    }

    [Fact]
    public async Task Move_WithZeroQuantity_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateMoveQuantityRequest(quantity: 0);
        _controller.ModelState.AddModelError("Quantity", "Quantity must be greater than 0");

        // Act
        var result = await _controller.Move(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}

