using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;

namespace quantity_move_api.Tests.Controllers;

public class BaseControllerTests
{
    private readonly Mock<ILogger<TestController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly TestController _controller;

    public BaseControllerTests()
    {
        _mockLogger = new Mock<ILogger<TestController>>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:DefaultWarehouse", "CONFIG_WAREHOUSE" },
            { "Defaults:DefaultSite", "CONFIG_SITE" }
        });
        _configuration = configBuilder.Build();
        
        _controller = new TestController(_mockLogger.Object, _configuration, _mockConfigurationService.Object);
    }

    [Fact]
    public void GetDefaultWarehouse_WithProvidedValue_ReturnsProvidedValue()
    {
        // Arrange
        var providedWarehouse = "PROVIDED_WAREHOUSE";

        // Act
        var result = _controller.TestGetDefaultWarehouse(providedWarehouse);

        // Assert
        result.Should().Be(providedWarehouse);
    }

    [Fact]
    public void GetDefaultWarehouse_WithNull_UsesConfigurationService()
    {
        // Arrange
        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("SERVICE_WAREHOUSE");

        // Act
        var result = _controller.TestGetDefaultWarehouse(null);

        // Assert
        result.Should().Be("SERVICE_WAREHOUSE");
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
    }

    [Fact]
    public void GetDefaultWarehouse_WithNullAndNoService_UsesConfiguration()
    {
        // Arrange
        var controllerWithoutService = new TestController(_mockLogger.Object, _configuration, null);

        // Act
        var result = controllerWithoutService.TestGetDefaultWarehouse(null);

        // Assert
        result.Should().Be("CONFIG_WAREHOUSE");
    }

    [Fact]
    public void GetDefaultWarehouse_WithNullAndNoConfig_UsesDefaultValue()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var controllerWithoutConfig = new TestController(_mockLogger.Object, emptyConfig, null);

        // Act
        var result = controllerWithoutConfig.TestGetDefaultWarehouse(null);

        // Assert
        result.Should().Be("MAIN");
    }

    [Fact]
    public void GetDefaultWarehouse_WithEmptyString_UsesFallback()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var controllerWithoutConfig = new TestController(_mockLogger.Object, emptyConfig, null);

        // Act
        var result = controllerWithoutConfig.TestGetDefaultWarehouse("");

        // Assert
        result.Should().Be("MAIN");
    }

    [Fact]
    public void GetDefaultWarehouse_WithWhitespace_UsesFallback()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var controllerWithoutConfig = new TestController(_mockLogger.Object, emptyConfig, null);

        // Act
        var result = controllerWithoutConfig.TestGetDefaultWarehouse("   ");

        // Assert
        result.Should().Be("MAIN");
    }

    [Fact]
    public void GetDefaultSite_WithProvidedValue_ReturnsProvidedValue()
    {
        // Arrange
        var providedSite = "PROVIDED_SITE";

        // Act
        var result = _controller.TestGetDefaultSite(providedSite);

        // Assert
        result.Should().Be(providedSite);
    }

    [Fact]
    public void GetDefaultSite_WithNull_UsesConfigurationService()
    {
        // Arrange
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SERVICE_SITE");

        // Act
        var result = _controller.TestGetDefaultSite(null);

        // Assert
        result.Should().Be("SERVICE_SITE");
        _mockConfigurationService.Verify(x => x.GetDefaultSite(), Times.Once);
    }

    [Fact]
    public void GetDefaultSite_WithNullAndNoService_UsesConfiguration()
    {
        // Arrange
        var controllerWithoutService = new TestController(_mockLogger.Object, _configuration, null);

        // Act
        var result = controllerWithoutService.TestGetDefaultSite(null);

        // Assert
        result.Should().Be("CONFIG_SITE");
    }

    [Fact]
    public void GetDefaultSite_WithNullAndNoConfig_UsesDefaultValue()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var controllerWithoutConfig = new TestController(_mockLogger.Object, emptyConfig, null);

        // Act
        var result = controllerWithoutConfig.TestGetDefaultSite(null);

        // Assert
        result.Should().Be("Default");
    }

    [Fact]
    public void HandleModelStateErrors_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("TestField", "Test error");

        // Act
        var result = _controller.TestHandleModelStateErrors<object>();

        // Assert
        result.Should().NotBeNull();
        result!.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest!.Value.Should().BeOfType<ApiResponse<object>>();
        
        var response = badRequest.Value as ApiResponse<object>;
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Test error");
    }

    [Fact]
    public void HandleModelStateErrors_WithValidModelState_ReturnsNull()
    {
        // Act
        var result = _controller.TestHandleModelStateErrors<object>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void HandleError_LogsErrorAndReturns500()
    {
        // Arrange
        var exception = new Exception("Test error");

        // Act
        var result = _controller.TestHandleError<object>(exception, "test operation");

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        
        var response = objectResult.Value as ApiResponse<object>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("test operation");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}

// Test controller to expose protected methods
public class TestController : BaseController
{
    public TestController(ILogger logger, IConfiguration configuration, IConfigurationService? configurationService)
        : base(logger, configuration, configurationService)
    {
    }

    public string TestGetDefaultWarehouse(string? providedWarehouse)
    {
        return GetDefaultWarehouse(providedWarehouse);
    }

    public string TestGetDefaultSite(string? providedSite)
    {
        return GetDefaultSite(providedSite);
    }

    public ActionResult<ApiResponse<T>>? TestHandleModelStateErrors<T>()
    {
        return HandleModelStateErrors<T>();
    }

    public ActionResult<ApiResponse<T>> TestHandleError<T>(Exception ex, string operation)
    {
        return HandleError<T>(ex, operation);
    }
}

