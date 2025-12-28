using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Middleware;
using quantity_move_api.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace quantity_move_api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
    private readonly ExceptionHandlingMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _middleware = new ExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WithSuccessfulRequest_DoesNotHandleException()
    {
        // Arrange
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(200);
        _mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedAccessException_Returns401()
    {
        // Arrange
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        _httpContext.Response.ContentType.Should().Be("application/json");
        
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Unauthorized access.");
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentException_Returns400()
    {
        // Arrange
        var exceptionMessage = "Invalid argument";
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(new ArgumentException(exceptionMessage));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");
        
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentNullException_Returns400()
    {
        // Arrange
        var exceptionMessage = "Argument cannot be null";
        var exception = new ArgumentNullException("param", exceptionMessage);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // ArgumentNullException.Message includes parameter name, so check that it contains the message
        apiResponse!.Message.Should().Contain(exceptionMessage);
    }

    [Fact]
    public async Task InvokeAsync_WithKeyNotFoundException_Returns404()
    {
        // Arrange
        var exceptionMessage = "Key not found";
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        apiResponse!.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_Returns500()
    {
        // Arrange
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("An error occurred while processing your request.");
    }

    [Fact]
    public async Task InvokeAsync_LogsException_WhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ResponseContainsExceptionMessage_InErrors()
    {
        // Arrange
        var exceptionMessage = "Test error message";
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        apiResponse!.Errors.Should().NotBeNull();
        apiResponse.Errors!.Should().Contain(exceptionMessage);
    }

    private async Task<string> GetResponseBody()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}

