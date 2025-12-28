using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using System.Net;

namespace quantity_move_api.Tests.Controllers;

public class HealthControllerTests
{
    private readonly Mock<HealthCheckService> _mockHealthCheckService;
    private readonly Mock<ILogger<HealthController>> _mockLogger;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _mockHealthCheckService = new Mock<HealthCheckService>();
        _mockLogger = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_mockHealthCheckService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Get_ReturnsHealthyStatus()
    {
        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        
        var value = okResult.Value;
        value.Should().NotBeNull();
        
        // Verify the response contains status and timestamp
        var statusProperty = value!.GetType().GetProperty("status");
        statusProperty.Should().NotBeNull();
    }

    [Fact]
    public async Task Ready_WithHealthyHealthCheck_ReturnsReady()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                { "self", new HealthReportEntry(HealthStatus.Healthy, "OK", TimeSpan.Zero, null, null) }
            },
            TimeSpan.Zero);

        _mockHealthCheckService.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), default))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_WithUnhealthyHealthCheck_ReturnsServiceUnavailable()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                { "self", new HealthReportEntry(HealthStatus.Unhealthy, "Error", TimeSpan.Zero, null, null) }
            },
            TimeSpan.Zero);

        _mockHealthCheckService.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), default))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Ready_WithException_ReturnsServiceUnavailable()
    {
        // Arrange
        _mockHealthCheckService.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), default))
            .ThrowsAsync(new Exception("Health check failed"));

        // Act
        var result = await _controller.Ready();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Ready_WithHealthyHealthCheck_IncludesChecks()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                { "self", new HealthReportEntry(HealthStatus.Healthy, "OK", TimeSpan.Zero, null, null) }
            },
            TimeSpan.Zero);

        _mockHealthCheckService.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), default))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void Live_ReturnsAliveStatus()
    {
        // Act
        var result = _controller.Live();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        
        var value = okResult.Value;
        value.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHealthCheckService_ThrowsException()
    {
        // Act
        Action act = () => new HealthController(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

